using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Features.Inventory.StockReceipts; // for ServiceResult
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BackEnd.Features.Customers;

public class CustomerService
{
    private readonly QuanLyCFDbContext _db;
    private readonly BackEnd.Shared.SmsService _sms;

    public CustomerService(QuanLyCFDbContext db, BackEnd.Shared.SmsService sms)
    {
        _db = db;
        _sms = sms;
    }

    public async Task<IEnumerable<CustomerListItem>> ListAsync(string? q, string? tier)
    {
        var query = _db.KhachHangs.AsQueryable();

        // 1. Tìm kiếm theo từ khóa (tên, SĐT, email)
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => 
                (x.HoTen != null && x.HoTen.ToLower().Contains(kw)) ||
                (x.SoDienThoai != null && x.SoDienThoai.Contains(kw)) ||
                (x.Email != null && x.Email.ToLower().Contains(kw))
            );
        }

        // 2. Lọc theo hạng thành viên (map từ tiếng Việt sang tiếng Anh lưu trong DB)
        if (!string.IsNullOrWhiteSpace(tier) && tier != "Tất cả")
        {
            var dbTier = MapTierToEn(tier);
            query = query.Where(x => x.HangThanhVien == dbTier);
        }

        var list = await query.OrderByDescending(x => x.DiemTichLuy).ToListAsync();
        var result = new List<CustomerListItem>();

        foreach (var x in list)
        {
            var visits = await _db.DonHangs.CountAsync(d => d.MaKhachHang == x.MaKhachHang);
            result.Add(new CustomerListItem(
                x.MaKhachHang,
                x.HoTen ?? "Khách hàng mới",
                x.SoDienThoai ?? "",
                x.Email,
                MapTierToVn(x.HangThanhVien),
                x.DiemTichLuy,
                x.TongTienDaTieu,
                x.LanGheThamCuoi.HasValue ? x.LanGheThamCuoi.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "Chưa ghé thăm",
                visits
            ));
        }

        return result;
    }

    public async Task<CustomerDetail?> GetByIdAsync(int id)
    {
        var x = await _db.KhachHangs.FindAsync(id);
        if (x == null) return null;

        var visits = await _db.DonHangs.CountAsync(d => d.MaKhachHang == x.MaKhachHang);
        
        var history = await _db.Set<LichSuDiem>()
            .Where(h => h.MaKhachHang == id)
            .OrderByDescending(h => h.ThoiGianTao)
            .Select(h => new CustomerHistoryItem(
                h.ThoiGianTao.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
                h.GhiChu ?? (h.LoaiBienDong == "Tich" ? "Tích điểm mua hàng" : "Đổi quà tặng"),
                h.SoDiem
            ))
            .ToListAsync();

        return new CustomerDetail(
            x.MaKhachHang,
            x.HoTen ?? "Khách hàng mới",
            x.SoDienThoai ?? "",
            x.Email,
            x.GhiChuKhachHang,
            MapTierToVn(x.HangThanhVien),
            x.DiemTichLuy,
            x.TongTienDaTieu,
            x.LanGheThamCuoi.HasValue ? x.LanGheThamCuoi.Value.ToLocalTime().ToString("dd/MM/yyyy HH:mm") : "Chưa ghé thăm",
            visits,
            history
        );
    }

    public async Task<ServiceResult<int>> CreateAsync(CreateCustomerRequest req)
    {
        var phone = req.Phone.Trim();
        if (await _db.KhachHangs.AnyAsync(x => x.SoDienThoai == phone))
        {
            return ServiceResult<int>.Fail("Số điện thoại này đã được đăng ký.");
        }

        if (!string.IsNullOrWhiteSpace(req.Email) && await _db.KhachHangs.AnyAsync(x => x.Email == req.Email.Trim()))
        {
            return ServiceResult<int>.Fail("Địa chỉ email này đã được sử dụng.");
        }

        var kh = new KhachHang
        {
            HoTen = req.Name.Trim(),
            SoDienThoai = phone,
            Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim(),
            GhiChuKhachHang = req.Note?.Trim(),
            HangThanhVien = "Member",
            DiemTichLuy = 0,
            TongTienDaTieu = 0,
            ThoiGianTao = DateTime.UtcNow
        };

        _db.KhachHangs.Add(kh);
        await _db.SaveChangesAsync();

        return ServiceResult<int>.Ok(kh.MaKhachHang);
    }

    public async Task<ServiceResult<bool>> UpdateAsync(int id, UpdateCustomerRequest req)
    {
        var kh = await _db.KhachHangs.FindAsync(id);
        if (kh == null) return ServiceResult<bool>.Fail("Không tìm thấy khách hàng.");

        var phone = req.Phone.Trim();
        if (await _db.KhachHangs.AnyAsync(x => x.SoDienThoai == phone && x.MaKhachHang != id))
        {
            return ServiceResult<bool>.Fail("Số điện thoại này đã thuộc về khách hàng khác.");
        }

        if (!string.IsNullOrWhiteSpace(req.Email) && await _db.KhachHangs.AnyAsync(x => x.Email == req.Email.Trim() && x.MaKhachHang != id))
        {
            return ServiceResult<bool>.Fail("Địa chỉ email này đã thuộc về khách hàng khác.");
        }

        kh.HoTen = req.Name.Trim();
        kh.SoDienThoai = phone;
        kh.Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim();
        kh.GhiChuKhachHang = req.Note?.Trim();

        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var kh = await _db.KhachHangs.FindAsync(id);
        if (kh == null) return ServiceResult<bool>.Fail("Không tìm thấy khách hàng.");

        // Nhờ cấu hình Cascade / SetNull trong DbContext, ta có thể xóa an toàn mà không lo khóa ngoại rác
        _db.KhachHangs.Remove(kh);
        await _db.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<IEnumerable<RewardDto>> GetRewardsAsync()
    {
        return await _db.Set<PhanThuong>()
            .Where(x => x.TrangThaiHoatDong)
            .OrderBy(x => x.DiemCanDoi)
            .Select(x => new RewardDto(x.MaPhanThuong, x.TenPhanThuong, x.DiemCanDoi, x.MoTa))
            .ToListAsync();
    }

    private static readonly ConcurrentDictionary<int, (string Otp, DateTime Expires)> _otpStore = new();

    public bool IsSmsSimulated()
    {
        return _sms.IsSimulated();
    }

    public async Task<string> GenerateOtpAsync(int customerId)
    {
        var kh = await _db.KhachHangs.FindAsync(customerId);
        var phone = kh?.SoDienThoai ?? "";

        var otp = new Random().Next(100000, 999999).ToString();
        _otpStore[customerId] = (otp, DateTime.UtcNow.AddMinutes(5));

        var msg = $"[BrewManager] Ma OTP xac nhan doi diem cua ban la: {otp}. Ma co hieu luc trong 5 phut.";

        if (!string.IsNullOrWhiteSpace(phone))
        {
            await _sms.SendSmsAsync(phone, msg);
        }

        Console.WriteLine($"\n[OTP SIMULATION] Mã OTP của khách hàng {customerId} là: {otp} (Hiệu lực 5 phút)\n");
        return otp;
    }

    public bool VerifyOtp(int customerId, string otp)
    {
        if (_otpStore.TryGetValue(customerId, out var data))
        {
            if (data.Expires >= DateTime.UtcNow && data.Otp == otp.Trim())
            {
                _otpStore.TryRemove(customerId, out _); // Xóa OTP sau khi dùng
                return true;
            }
        }
        return false;
    }

    public async Task<ServiceResult<int>> RedeemRewardAsync(int id, int rewardId, string otp)
    {
        if (string.IsNullOrWhiteSpace(otp) || !VerifyOtp(id, otp))
            return ServiceResult<int>.Fail("Mã OTP không chính xác hoặc đã hết hạn.");

        var kh = await _db.KhachHangs.FindAsync(id);
        if (kh == null) return ServiceResult<int>.Fail("Không tìm thấy khách hàng.");

        var reward = await _db.Set<PhanThuong>().FindAsync(rewardId);
        if (reward == null || !reward.TrangThaiHoatDong) 
            return ServiceResult<int>.Fail("Phần thưởng không tồn tại hoặc đã ngưng hoạt động.");

        if (kh.DiemTichLuy < reward.DiemCanDoi)
            return ServiceResult<int>.Fail($"Khách hàng không đủ điểm (Cần {reward.DiemCanDoi} điểm, hiện có {kh.DiemTichLuy} điểm).");

        // Khấu trừ điểm
        kh.DiemTichLuy -= reward.DiemCanDoi;
        
        // Cập nhật hạng sau khi đổi điểm
        kh.HangThanhVien = GetTierByPoints(kh.DiemTichLuy);

        // Lưu lịch sử biến động điểm
        var ls = new LichSuDiem
        {
            MaKhachHang = id,
            LoaiBienDong = "Doi",
            SoDiem = -reward.DiemCanDoi,
            GhiChu = $"Đổi thưởng: {reward.TenPhanThuong}",
            ThoiGianTao = DateTime.UtcNow
        };
        _db.Set<LichSuDiem>().Add(ls);

        await _db.SaveChangesAsync();

        return ServiceResult<int>.Ok(kh.DiemTichLuy);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static string GetTierByPoints(int points)
    {
        if (points >= 3000) return "Diamond";
        if (points >= 1500) return "Gold";
        if (points >= 500) return "Silver";
        return "Member";
    }

    private static string MapTierToVn(string tier)
    {
        return tier switch
        {
            "Diamond" => "Kim cương",
            "Gold" => "Vàng",
            "Silver" => "Bạc",
            "Member" => "Đồng",
            _ => tier
        };
    }

    private static string MapTierToEn(string tier)
    {
        return tier switch
        {
            "Kim cương" => "Diamond",
            "Vàng" => "Gold",
            "Bạc" => "Silver",
            "Đồng" => "Member",
            _ => tier
        };
    }
}
