using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Storefront;

public class CustomerService
{
    private readonly QuanLyCFDbContext _db;
    public CustomerService(QuanLyCFDbContext db) => _db = db;

    // ─── Danh sách khách hàng (admin) ──────────────────────────────────
    public async Task<List<CustomerDto>> GetAllAsync(string? search = null)
    {
        var query = _db.KhachHangs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim().ToLower();
            query = query.Where(k =>
                (k.HoTen != null && k.HoTen.ToLower().Contains(search)) ||
                (k.SoDienThoai != null && k.SoDienThoai.Contains(search)));
        }

        var list = await query
            .OrderByDescending(k => k.ThoiGianTao)
            .ToListAsync();

        return list.Select(k => Map(k, false)).ToList();
    }

    // ─── Tìm theo số điện thoại ─────────────────────────────────
    public async Task<CustomerDto?> GetByPhoneAsync(string phone)
    {
        phone = phone.Trim();
        var kh = await _db.KhachHangs.FirstOrDefaultAsync(k => k.SoDienThoai == phone);
        return kh is null ? null : Map(kh, false);
    }
    public async Task<(CustomerDto? Data, string? Error)> LoginAsync(CustomerLoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.SoDienThoai))
            return (null, "Số điện thoại không được để trống.");

        if (string.IsNullOrWhiteSpace(req.HoTen))
            return (null, "Họ và tên không được để trống.");

        var phone = req.SoDienThoai.Trim();
        // Loại bỏ khoảng trắng, dấu chấm, gạch ngang nếu có
        phone = phone.Replace(" ", "").Replace(".", "").Replace("-", "");

        if (phone.Length < 9 || phone.Length > 12)
            return (null, "Số điện thoại không hợp lệ.");

        // Kiểm tra xem khách hàng đã tồn tại chưa
        var kh = await _db.KhachHangs.FirstOrDefaultAsync(x => x.SoDienThoai == phone);
        bool isNew;
        if (kh == null)
        {
            isNew = true;
            // Đăng ký mới
            kh = new KhachHang
            {
                SoDienThoai = phone,
                HoTen = req.HoTen.Trim(),
                HangThanhVien = "Member",
                DiemTichLuy = 0,
                TongTienDaTieu = 0,
                LanGheThamCuoi = DateTime.UtcNow,
                ThoiGianTao = DateTime.UtcNow
            };
            _db.KhachHangs.Add(kh);
        }
        else
        {
            isNew = false;
            // Cập nhật lần ghé thăm cuối và cập nhật tên nếu thay đổi
            kh.LanGheThamCuoi = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(req.HoTen) && kh.HoTen != req.HoTen.Trim())
            {
                kh.HoTen = req.HoTen.Trim();
            }
        }

        await _db.SaveChangesAsync();
        return (Map(kh, isNew), null);
    }

    private static CustomerDto Map(KhachHang kh, bool isNew) => new(
        kh.MaKhachHang,
        kh.HoTen,
        kh.SoDienThoai,
        kh.HangThanhVien,
        kh.DiemTichLuy,
        kh.TongTienDaTieu,
        kh.LanGheThamCuoi,
        kh.ThoiGianTao,
        isNew
    );
}
