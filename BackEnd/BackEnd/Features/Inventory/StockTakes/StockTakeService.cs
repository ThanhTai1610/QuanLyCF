using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts; // dùng lại ServiceResult<T>
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.StockTakes;

public class StockTakeService
{
    private readonly QuanLyCFDbContext _db;
    public StockTakeService(QuanLyCFDbContext db) => _db = db;

    /// <summary>Tạo phiếu kiểm kê: chốt tồn hệ thống vào SoLuong, lưu số đếm thực tế. Trạng thái ChoDuyet.</summary>
    public async Task<ServiceResult<int>> TaoAsync(CreateStockTakeRequest req, int maNhanVien)
    {
        if (req.ChiTiets is null || req.ChiTiets.Count == 0)
            return ServiceResult<int>.Fail("Phiếu kiểm kê phải có ít nhất 1 mặt hàng.");

        var ids = req.ChiTiets.Select(l => l.MaNguyenLieu).Distinct().ToList();
        var materials = await _db.NguyenLieus.Where(x => ids.Contains(x.MaNguyenLieu)).ToDictionaryAsync(x => x.MaNguyenLieu);
        if (materials.Count != ids.Count)
            return ServiceResult<int>.Fail("Có nguyên liệu không tồn tại.");

        var phieu = new PhieuKho
        {
            LoaiPhieu = "KiemKe",
            MaNhanVien = maNhanVien,
            TrangThai = "ChoDuyet",
            GhiChu = req.GhiChu,
            ThoiGianTao = DateTime.UtcNow,
            ChiTiets = req.ChiTiets.Select(l => new ChiTietPhieuKho
            {
                MaNguyenLieu = l.MaNguyenLieu,
                SoLuong = materials[l.MaNguyenLieu].SoLuongTon,   // tồn hệ thống tại thời điểm kiểm
                SoLuongThucTe = l.SoLuongThucTe,
                LyDoLech = l.LyDoLech,
            }).ToList(),
        };
        _db.PhieuKhos.Add(phieu);
        await _db.SaveChangesAsync();
        return ServiceResult<int>.Ok(phieu.MaPhieu);
    }

    /// <summary>Duyệt phiếu kiểm kê: cập nhật tồn = số đếm thực tế.</summary>
    public async Task<ServiceResult<bool>> DuyetAsync(int maPhieu)
    {
        var phieu = await _db.PhieuKhos.Include(x => x.ChiTiets).ThenInclude(c => c.NguyenLieu)
            .FirstOrDefaultAsync(x => x.MaPhieu == maPhieu && x.LoaiPhieu == "KiemKe");
        if (phieu is null) return ServiceResult<bool>.Fail("Không tìm thấy phiếu kiểm kê.");
        if (phieu.TrangThai != "ChoDuyet") return ServiceResult<bool>.Fail("Phiếu đã được xử lý.");

        foreach (var ct in phieu.ChiTiets)
            if (ct.SoLuongThucTe is { } thuc)
                ct.NguyenLieu.SoLuongTon = thuc;

        phieu.TrangThai = "DaDuyet";
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> TuChoiAsync(int maPhieu)
    {
        var phieu = await _db.PhieuKhos.FirstOrDefaultAsync(x => x.MaPhieu == maPhieu && x.LoaiPhieu == "KiemKe");
        if (phieu is null) return ServiceResult<bool>.Fail("Không tìm thấy phiếu kiểm kê.");
        if (phieu.TrangThai != "ChoDuyet") return ServiceResult<bool>.Fail("Phiếu đã được xử lý.");
        phieu.TrangThai = "TuChoi";
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
