using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Invoices;

public class InvoiceService
{
    private readonly QuanLyCFDbContext _db;

    public InvoiceService(QuanLyCFDbContext db)
    {
        _db = db;
    }

    public async Task<List<InvoiceListItemDto>> LayDanhSachHoaDonAsync(InvoiceQuery query)
    {
        var hoaDons = _db.HoaDons
            .AsNoTracking()
            .Include(h => h.DonHang).ThenInclude(d => d.Ban)
            .Include(h => h.NhanVienThuNgan)
            .Include(h => h.ChiTietThanhToans)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var keyword = query.Search.Trim();
            hoaDons = hoaDons.Where(h =>
                h.MaHoaDon.ToString().Contains(keyword) ||
                h.MaDonHang.ToString().Contains(keyword) ||
                (h.DonHang.Ban != null && h.DonHang.Ban.TenBan.Contains(keyword)));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
            hoaDons = hoaDons.Where(h => h.TrangThai == query.TrangThai);

        if (query.TuNgay.HasValue)
            hoaDons = hoaDons.Where(h => h.ThoiGianThanhToan >= query.TuNgay.Value.Date);

        if (query.DenNgay.HasValue)
        {
            var denNgay = query.DenNgay.Value.Date.AddDays(1);
            hoaDons = hoaDons.Where(h => h.ThoiGianThanhToan < denNgay);
        }

        return await hoaDons
            .OrderByDescending(h => h.ThoiGianThanhToan)
            .Select(h => new InvoiceListItemDto(
                h.MaHoaDon,
                h.MaDonHang,
                h.DonHang.LoaiDonHang == "TakeAway" ? "Mang về" : h.DonHang.Ban != null ? h.DonHang.Ban.TenBan : null,
                h.ThoiGianThanhToan,
                h.TongThanhTien,
                h.ChiTietThanhToans.OrderBy(ct => ct.MaThanhToan).Select(ct => ct.PhuongThuc).FirstOrDefault(),
                h.NhanVienThuNgan != null ? h.NhanVienThuNgan.HoTen : null,
                h.TrangThai
            ))
            .ToListAsync();
    }

    public async Task<InvoiceDetailDto?> LayChiTietHoaDonAsync(int id)
    {
        var hd = await _db.HoaDons
            .AsNoTracking()
            .Include(h => h.DonHang).ThenInclude(d => d.Ban)
            .Include(h => h.DonHang).ThenInclude(d => d.ChiTiets).ThenInclude(ct => ct.SanPham)
            .Include(h => h.DonHang).ThenInclude(d => d.ChiTiets).ThenInclude(ct => ct.KichCo)
            .Include(h => h.NhanVienThuNgan)
            .Include(h => h.ChiTietThanhToans)
            .FirstOrDefaultAsync(h => h.MaHoaDon == id);

        if (hd == null) return null;

        var phuongThuc = hd.ChiTietThanhToans.OrderBy(ct => ct.MaThanhToan).Select(ct => ct.PhuongThuc).FirstOrDefault();
        var tenBan = hd.DonHang.LoaiDonHang == "TakeAway" ? "Mang về" : hd.DonHang.Ban?.TenBan;
        var items = hd.DonHang.ChiTiets.Select(ct => new InvoiceItemDto(
            ct.SanPham?.TenSanPham + (ct.KichCo != null ? $" ({ct.KichCo.TenKichCo})" : ""),
            ct.SoLuong,
            ct.DonGia,
            ct.ThanhTien
        )).ToList();

        return new InvoiceDetailDto(
            hd.MaHoaDon,
            hd.MaDonHang,
            tenBan,
            hd.ThoiGianThanhToan,
            hd.TongThanhTien,
            phuongThuc,
            hd.NhanVienThuNgan?.HoTen,
            hd.TrangThai,
            hd.DonHang.TongTienHang,
            hd.DonHang.TienGiamGia,
            hd.DonHang.PhiDichVu,
            hd.DonHang.ThueVAT,
            hd.SoTienKhachTra,
            hd.TienThoiLai,
            hd.MaSoThueXuatHD,
            items
        );
    }
}