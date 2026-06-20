using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Promotions;

public class PromotionService
{
    private static readonly string[] LoaiHopLe = { "PhanTram", "TienMat" };

    private readonly QuanLyCFDbContext _db;
    public PromotionService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<PromotionItem>> ListAsync()
    {
        var list = await _db.KhuyenMais.OrderByDescending(k => k.MaKhuyenMai).ToListAsync();
        return list.Select(Map).ToList();
    }

    /// <summary>Khuyến mãi còn hiệu lực (đang bật, trong hạn, còn lượt) — cho POS chọn.</summary>
    public async Task<List<PromotionItem>> ActiveAsync()
    {
        var now = DateTime.UtcNow;
        var list = await _db.KhuyenMais
            .Where(k => k.TrangThaiHoatDong
                && (k.NgayBatDau == null || k.NgayBatDau <= now)
                && (k.NgayKetThuc == null || k.NgayKetThuc >= now)
                && (k.SoLuongGioiHan == null || k.SoLuongDaDung < k.SoLuongGioiHan))
            .OrderBy(k => k.TenChuongTrinh).ToListAsync();
        return list.Select(Map).ToList();
    }

    public async Task<(int Id, string? Error)> CreateAsync(SavePromotionRequest req)
    {
        var err = Validate(req);
        if (err != null) return (0, err);
        if (!string.IsNullOrWhiteSpace(req.MaGiamGia) &&
            await _db.KhuyenMais.AnyAsync(k => k.MaGiamGia == req.MaGiamGia!.Trim()))
            return (0, "Mã giảm giá đã tồn tại.");

        var km = new KhuyenMai();
        ApplyFields(km, req);
        _db.KhuyenMais.Add(km);
        await _db.SaveChangesAsync();
        return (km.MaKhuyenMai, null);
    }

    public async Task<(bool Ok, string? Error)> UpdateAsync(int id, SavePromotionRequest req)
    {
        var km = await _db.KhuyenMais.FindAsync(id);
        if (km is null) return (false, "Không tìm thấy khuyến mãi.");
        var err = Validate(req);
        if (err != null) return (false, err);
        if (!string.IsNullOrWhiteSpace(req.MaGiamGia) &&
            await _db.KhuyenMais.AnyAsync(k => k.MaGiamGia == req.MaGiamGia!.Trim() && k.MaKhuyenMai != id))
            return (false, "Mã giảm giá đã tồn tại.");

        ApplyFields(km, req);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> DeleteAsync(int id)
    {
        var km = await _db.KhuyenMais.FindAsync(id);
        if (km is null) return (false, "Không tìm thấy khuyến mãi.");
        if (await _db.DonHangs.AnyAsync(d => d.MaKhuyenMai == id))
            return (false, "Khuyến mãi đã dùng trong đơn hàng, không thể xoá. Hãy tắt thay vì xoá.");
        _db.KhuyenMais.Remove(km);
        await _db.SaveChangesAsync();
        return (true, null);
    }

    /// <summary>Tìm khuyến mãi theo mã/chương trình, kiểm tra điều kiện, tính tiền giảm (chưa lưu).</summary>
    public async Task<(ApplyPromotionResult? Data, string? Error)> ApplyAsync(ApplyPromotionRequest req)
    {
        var (km, err) = await TimVaKiemTraAsync(req.MaKhuyenMai, req.Code, req.TongTien);
        if (err != null) return (null, err);
        var giam = TinhGiam(km!, req.TongTien);
        return (new ApplyPromotionResult(km!.MaKhuyenMai, km.TenChuongTrinh, giam, req.TongTien - giam), null);
    }

    /// <summary>Dùng khi thanh toán: kiểm tra + tính giảm + tăng lượt đã dùng. Trả về (km, tienGiam, error).</summary>
    public async Task<(KhuyenMai? Km, decimal TienGiam, string? Error)> ApDungChoDonAsync(int maKhuyenMai, decimal tongTien)
    {
        var (km, err) = await TimVaKiemTraAsync(maKhuyenMai, null, tongTien);
        if (err != null) return (null, 0, err);
        var giam = TinhGiam(km!, tongTien);
        km!.SoLuongDaDung++;
        return (km, giam, null);
    }

    // ── Private ──
    private async Task<(KhuyenMai? Km, string? Error)> TimVaKiemTraAsync(int? maKhuyenMai, string? code, decimal tongTien)
    {
        KhuyenMai? km = null;
        if (maKhuyenMai is { } id) km = await _db.KhuyenMais.FindAsync(id);
        else if (!string.IsNullOrWhiteSpace(code)) km = await _db.KhuyenMais.FirstOrDefaultAsync(k => k.MaGiamGia == code.Trim());
        else return (null, "Thiếu mã hoặc chương trình khuyến mãi.");
        if (km is null) return (null, "Không tìm thấy khuyến mãi.");

        var now = DateTime.UtcNow;
        if (!km.TrangThaiHoatDong) return (null, "Khuyến mãi không còn hiệu lực.");
        if (km.NgayBatDau is { } bd && now < bd) return (null, "Chưa tới thời gian áp dụng.");
        if (km.NgayKetThuc is { } kt && now > kt) return (null, "Khuyến mãi đã hết hạn.");
        if (km.SoLuongGioiHan is { } gh && km.SoLuongDaDung >= gh) return (null, "Khuyến mãi đã hết lượt.");
        if (km.DonToiThieu is { } dtt && tongTien < dtt) return (null, $"Đơn tối thiểu {dtt:n0}đ mới áp dụng được.");
        return (km, null);
    }

    private static decimal TinhGiam(KhuyenMai km, decimal tongTien)
    {
        var giam = km.LoaiGiamGia == "PhanTram" ? tongTien * km.GiaTriGiam / 100m : km.GiaTriGiam;
        if (km.GiamToiDa is { } max && giam > max) giam = max;
        if (giam > tongTien) giam = tongTien;
        return Math.Round(giam, 0);
    }

    private static string? Validate(SavePromotionRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.TenChuongTrinh)) return "Vui lòng nhập tên chương trình.";
        if (!LoaiHopLe.Contains(req.LoaiGiamGia)) return "Loại giảm giá không hợp lệ.";
        if (req.GiaTriGiam <= 0) return "Giá trị giảm phải lớn hơn 0.";
        if (req.LoaiGiamGia == "PhanTram" && req.GiaTriGiam > 100) return "Phần trăm giảm tối đa 100%.";
        if (req.NgayBatDau is { } bd && req.NgayKetThuc is { } kt && kt < bd) return "Ngày kết thúc phải sau ngày bắt đầu.";
        return null;
    }

    private static void ApplyFields(KhuyenMai km, SavePromotionRequest req)
    {
        km.MaGiamGia = string.IsNullOrWhiteSpace(req.MaGiamGia) ? null : req.MaGiamGia.Trim().ToUpper();
        km.TenChuongTrinh = req.TenChuongTrinh.Trim();
        km.LoaiGiamGia = req.LoaiGiamGia;
        km.GiaTriGiam = req.GiaTriGiam;
        km.GiamToiDa = req.GiamToiDa;
        km.DonToiThieu = req.DonToiThieu;
        km.SoLuongGioiHan = req.SoLuongGioiHan;
        km.NgayBatDau = req.NgayBatDau;
        km.NgayKetThuc = req.NgayKetThuc;
        km.MoTa = req.MoTa;
        km.TrangThaiHoatDong = req.TrangThaiHoatDong;
    }

    private static PromotionItem Map(KhuyenMai k) => new(
        k.MaKhuyenMai, k.MaGiamGia, k.TenChuongTrinh, k.LoaiGiamGia, k.GiaTriGiam, k.GiamToiDa,
        k.DonToiThieu, k.SoLuongGioiHan, k.SoLuongDaDung, k.NgayBatDau, k.NgayKetThuc, k.MoTa, k.TrangThaiHoatDong);
}
