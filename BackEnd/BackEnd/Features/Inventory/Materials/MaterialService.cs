using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.Materials;

public class MaterialService
{
    private readonly QuanLyCFDbContext _db;
    public MaterialService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<MaterialItem>> LayDanhSachAsync(string? q, string? typeFilter, string? statusFilter)
    {
        var query = _db.NguyenLieus.AsQueryable();
        
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.TenNguyenLieu.ToLower().Contains(kw) || (x.MaVach_SKU != null && x.MaVach_SKU.ToLower().Contains(kw)));
        }

        if (!string.IsNullOrWhiteSpace(typeFilter) && typeFilter != "all")
        {
            query = query.Where(x => x.PhanLoai == typeFilter);
        }

        var items = await query.OrderBy(x => x.TenNguyenLieu).ToListAsync();
        var data = items.Select(Map).ToList();

        if (!string.IsNullOrWhiteSpace(statusFilter) && statusFilter != "all")
        {
            var mapStatus = statusFilter == "ok" ? "Ok" : statusFilter == "low" ? "SapHet" : "Het";
            data = data.Where(x => x.TrangThaiTon == mapStatus).ToList();
        }

        return data;
    }

    public async Task<object> TinhThongKeAsync()
    {
        var items = await _db.NguyenLieus.ToListAsync();
        return new
        {
            tongSKU = items.Count,
            sapHet = items.Count(x => TinhTrangThai(x) == "SapHet"),
            daHet = items.Count(x => TinhTrangThai(x) == "Het"),
        };
    }

    public async Task<ServiceResult<MaterialItem>> TaoAsync(SaveMaterialRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) && await _db.NguyenLieus.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU))
            return ServiceResult<MaterialItem>.Fail("Mã SKU đã tồn tại.");

        var nl = new NguyenLieu
        {
            TenNguyenLieu = req.TenNguyenLieu.Trim(),
            MaVach_SKU = req.MaVach_SKU,
            PhanLoai = req.PhanLoai,
            DonViTinh = req.DonViTinh,
            MucTonToiThieu = req.MucTonToiThieu,
            MucTonToiDa = req.MucTonToiDa,
            HanSuDungNgay = req.HanSuDungNgay,
            NgayHetHan = req.NgayHetHan,
            HinhAnh = req.HinhAnh,
            SoLuongTon = 0,
        };
        _db.NguyenLieus.Add(nl);
        await _db.SaveChangesAsync();
        return ServiceResult<MaterialItem>.Ok(Map(nl));
    }

    public async Task<ServiceResult<bool>> CapNhatAsync(int id, SaveMaterialRequest req)
    {
        var nl = await _db.NguyenLieus.FindAsync(id);
        if (nl is null) return ServiceResult<bool>.Fail("Không tìm thấy nguyên liệu.");

        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) && await _db.NguyenLieus.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU && x.MaNguyenLieu != id))
            return ServiceResult<bool>.Fail("Mã SKU đã tồn tại.");

        nl.TenNguyenLieu = req.TenNguyenLieu.Trim();
        nl.MaVach_SKU = req.MaVach_SKU;
        nl.PhanLoai = req.PhanLoai;
        nl.DonViTinh = req.DonViTinh;
        nl.MucTonToiThieu = req.MucTonToiThieu;
        nl.MucTonToiDa = req.MucTonToiDa;
        nl.HanSuDungNgay = req.HanSuDungNgay;
        nl.NgayHetHan = req.NgayHetHan;
        nl.HinhAnh = req.HinhAnh;
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> XoaAsync(int id)
    {
        var nl = await _db.NguyenLieus.FindAsync(id);
        if (nl is null) return ServiceResult<bool>.Fail("Không tìm thấy nguyên liệu.");

        if (await _db.ChiTietPhieuKhos.AnyAsync(x => x.MaNguyenLieu == id))
            return ServiceResult<bool>.Fail("Nguyên liệu đã phát sinh phiếu kho, không thể xoá.");

        _db.NguyenLieus.Remove(nl);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> DieuChinhNhanhAsync(int id, decimal soLuongThucTe, string lyDo, int maNhanVien)
    {
        var nl = await _db.NguyenLieus.FindAsync(id);
        if (nl is null) return ServiceResult<bool>.Fail("Không tìm thấy nguyên liệu.");
        if (soLuongThucTe < 0) return ServiceResult<bool>.Fail("Số lượng không được âm.");

        // Tạo phiếu kho loại "DieuChinh"
        var phieu = new PhieuKho
        {
            LoaiPhieu = "DieuChinh",
            MaNhanVien = maNhanVien,
            TrangThai = "DaDuyet",
            GhiChu = lyDo,
            ThoiGianTao = DateTime.UtcNow,
            ChiTiets = new List<ChiTietPhieuKho>
            {
                new ChiTietPhieuKho
                {
                    MaNguyenLieu = id,
                    SoLuong = nl.SoLuongTon,           // Tồn cũ
                    SoLuongThucTe = soLuongThucTe,     // Tồn mới
                    LyDoLech = lyDo
                }
            }
        };

        nl.SoLuongTon = soLuongThucTe; // Cập nhật số tồn
        _db.PhieuKhos.Add(phieu);
        await _db.SaveChangesAsync();
        
        return ServiceResult<bool>.Ok(true);
    }

    private static string TinhTrangThai(NguyenLieu x)
    {
        if (x.SoLuongTon <= 0) return "Het";
        if (x.MucTonToiThieu is { } min && x.SoLuongTon <= min) return "SapHet";
        return "Ok";
    }

    public static MaterialItem Map(NguyenLieu x) => new(
        x.MaNguyenLieu, x.TenNguyenLieu, x.MaVach_SKU, x.PhanLoai, x.DonViTinh,
        x.SoLuongTon, x.MucTonToiThieu, x.GiaVonTrungBinh, TinhTrangThai(x), x.NgayHetHan);
}
