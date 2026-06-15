using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.StockReceipts;

public record ServiceResult<T>(T? Data, string? Error)
{
    public static ServiceResult<T> Ok(T d) => new(d, null);
    public static ServiceResult<T> Fail(string e) => new(default, e);
}

public class StockReceiptService
{
    private readonly QuanLyCFDbContext _db;
    public StockReceiptService(QuanLyCFDbContext db) => _db = db;

    /// <summary>
    /// Lập phiếu nhập: tạo phiếu + tăng tồn + cập nhật giá vốn BÌNH QUÂN GIA QUYỀN + ghi công nợ NCC.
    /// Công thức: giáVốnMới = (tồnCũ*giáVốnCũ + sốNhập*đơnGiáNhập) / (tồnCũ + sốNhập)
    /// </summary>
    public async Task<ServiceResult<int>> NhapKhoAsync(CreateReceiptRequest req, int maNhanVien)
    {
        if (req.ChiTiets is null || req.ChiTiets.Count == 0)
            return ServiceResult<int>.Fail("Phiếu nhập phải có ít nhất 1 dòng hàng.");
        if (req.ChiTiets.Any(l => l.SoLuong <= 0 || l.DonGia < 0))
            return ServiceResult<int>.Fail("Số lượng phải > 0 và đơn giá không âm.");

        var ids = req.ChiTiets.Select(l => l.MaNguyenLieu).Distinct().ToList();
        var materials = await _db.NguyenLieus.Where(x => ids.Contains(x.MaNguyenLieu)).ToDictionaryAsync(x => x.MaNguyenLieu);
        if (materials.Count != ids.Count)
            return ServiceResult<int>.Fail("Có nguyên liệu không tồn tại.");
        if (req.MaNhaCungCap is { } sup && !await _db.NhaCungCaps.AnyAsync(x => x.MaNhaCungCap == sup))
            return ServiceResult<int>.Fail("Nhà cung cấp không tồn tại.");

        var tong = req.ChiTiets.Sum(l => l.SoLuong * l.DonGia);
        var daTra = Math.Min(req.TienDaThanhToan, tong);

        var phieu = new PhieuKho
        {
            LoaiPhieu = "NhapKho",
            MaNhanVien = maNhanVien,
            MaNhaCungCap = req.MaNhaCungCap,
            TongTienHang = tong,
            TienDaThanhToan = daTra,
            TrangThaiThanhToan = daTra >= tong ? "DaThanhToan" : daTra <= 0 ? "ChuaThanhToan" : "ThanhToanMotPhan",
            TrangThai = "DaHoanTat",
            GhiChu = req.GhiChu,
            ThoiGianTao = DateTime.UtcNow,
            ChiTiets = req.ChiTiets.Select(l => new ChiTietPhieuKho
            {
                MaNguyenLieu = l.MaNguyenLieu, SoLuong = l.SoLuong, DonGia = l.DonGia
            }).ToList(),
        };
        _db.PhieuKhos.Add(phieu);

        // Cập nhật tồn + giá vốn bình quân
        foreach (var line in req.ChiTiets)
        {
            var nl = materials[line.MaNguyenLieu];
            var tonCu = nl.SoLuongTon;
            var giaCu = nl.GiaVonTrungBinh ?? 0;
            var tonMoi = tonCu + line.SoLuong;
            nl.GiaVonTrungBinh = tonMoi > 0
                ? Math.Round((tonCu * giaCu + line.SoLuong * line.DonGia) / tonMoi, 2)
                : giaCu;
            nl.SoLuongTon = tonMoi;
        }

        // Ghi công nợ NCC (phần chưa trả)
        if (req.MaNhaCungCap is { } maNcc)
        {
            var ncc = await _db.NhaCungCaps.FindAsync(maNcc);
            if (ncc is not null) ncc.CongNoHienTai += (tong - daTra);
        }

        await _db.SaveChangesAsync();
        return ServiceResult<int>.Ok(phieu.MaPhieu);
    }
}
