using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Features.Inventory.StockReceipts; // for ServiceResult
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BackEnd.Features.Customers;

public class CustomerService
{
    private readonly QuanLyCFDbContext _db;
    private readonly BackEnd.Shared.EmailService _email;

    public CustomerService(QuanLyCFDbContext db, BackEnd.Shared.EmailService email)
    {
        _db = db;
        _email = email;
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

    public async Task<object?> GetByPhoneAsync(string phone, string? alternativePhone)
    {
        var x = await _db.KhachHangs.FirstOrDefaultAsync(c => c.SoDienThoai == phone || (alternativePhone != null && c.SoDienThoai == alternativePhone));
        if (x == null) return null;

        return new
        {
            id = x.MaKhachHang,
            name = x.HoTen ?? "Khách hàng mới",
            phone = x.SoDienThoai ?? "",
            tier = MapTierToVn(x.HangThanhVien),
            points = x.DiemTichLuy
        };
    }

    public async Task<object?> GetByEmailAsync(string email)
    {
        var x = await _db.KhachHangs.FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == email.Trim().ToLower());
        if (x == null) return null;

        return new
        {
            id = x.MaKhachHang,
            name = x.HoTen ?? "Khách hàng mới",
            phone = x.SoDienThoai ?? "",
            email = x.Email ?? "",
            tier = MapTierToVn(x.HangThanhVien),
            points = x.DiemTichLuy
        };
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


    public async Task<string> GenerateOtpAsync(int customerId)
    {
        var kh = await _db.KhachHangs.FindAsync(customerId);
        if (kh == null) throw new InvalidOperationException("Không tìm thấy khách hàng.");
        
        var email = kh.Email?.Trim() ?? "";
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new InvalidOperationException("Khách hàng chưa đăng ký địa chỉ email. Vui lòng cập nhật email của khách hàng trước khi đổi quà.");
        }

        var otp = new Random().Next(100000, 999999).ToString();
        _otpStore[customerId] = (otp, DateTime.UtcNow.AddMinutes(5));

        var subject = "[F6 Coffee] Mã OTP xác nhận đổi quà";
        var body = $@"
            <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                <h2 style='color: #CC8033; text-align: center;'>F6 Coffee Loyalty</h2>
                <p>Xin chào <strong>{kh.HoTen}</strong>,</p>
                <p>Hệ thống nhận được yêu cầu đổi điểm tích lũy của bạn tại cửa hàng <strong>F6 Coffee</strong>.</p>
                <div style='background-color: #fcf6f0; border: 1px dashed #CC8033; padding: 15px; border-radius: 5px; text-align: center; margin: 20px 0;'>
                    <span style='font-size: 14px; color: #666; display: block; margin-bottom: 5px;'>MÃ OTP CỦA BẠN LÀ:</span>
                    <strong style='font-size: 32px; color: #CC8033; letter-spacing: 5px;'>{otp}</strong>
                </div>
                <p style='color: #666; font-size: 13px;'>Mã này có hiệu lực trong vòng <strong>5 phút</strong>. Vui lòng không chia sẻ mã này cho bất kỳ ai.</p>
                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;' />
                <p style='font-size: 11px; color: #999; text-align: center;'>Đây là email tự động từ hệ thống F6 Coffee. Vui lòng không phản hồi email này.</p>
            </div>";

        await _email.SendEmailAsync(email, subject, body);

        Console.WriteLine($"\n[OTP EMAIL SIMULATION] Mã OTP của khách hàng {customerId} ({email}) là: {otp} (Hiệu lực 5 phút)\n");
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
        var kh = await _db.KhachHangs.FindAsync(id);
        if (kh == null) return ServiceResult<int>.Fail("Không tìm thấy khách hàng.");

        if (string.IsNullOrWhiteSpace(otp) || !VerifyOtp(id, otp))
            return ServiceResult<int>.Fail("Mã OTP không chính xác hoặc đã hết hạn.");

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

    public async Task<ServiceResult<int>> RedeemPointsPublicAsync(int id, int points, int? maDonHang)
    {
        var kh = await _db.KhachHangs.FindAsync(id);
        if (kh == null) return ServiceResult<int>.Fail("Không tìm thấy khách hàng.");

        if (kh.DiemTichLuy < points)
            return ServiceResult<int>.Fail($"Khách hàng không đủ điểm (Cần {points} điểm, hiện có {kh.DiemTichLuy} điểm).");

        kh.DiemTichLuy -= points;
        kh.HangThanhVien = GetTierByPoints(kh.DiemTichLuy);

        var ls = new LichSuDiem
        {
            MaKhachHang = id,
            LoaiBienDong = "Doi",
            SoDiem = -points,
            GhiChu = $"Đổi {points} điểm thưởng lấy chiết khấu đơn hàng tại bàn",
            ThoiGianTao = DateTime.UtcNow
        };
        _db.Set<LichSuDiem>().Add(ls);

        if (maDonHang is { } mdh)
        {
            var don = await _db.DonHangs.FindAsync(mdh);
            if (don != null)
            {
                don.MaKhachHang = id; // Link customer to order
                don.TienGiamGia += 20000; // Deduct 20,000 VND
                don.ThanhTien = Math.Max(0, don.ThanhTien - 20000);
            }
        }

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
