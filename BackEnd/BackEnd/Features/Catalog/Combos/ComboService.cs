using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts; // For ServiceResult
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Catalog.Combos;

public class ComboService
{
    private readonly QuanLyCFDbContext _db;
    public ComboService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<ComboListItem>> LayDanhSachAsync()
    {
        return await _db.Combos.OrderBy(x => x.TenCombo)
            .Select(x => new ComboListItem(
                x.MaCombo, x.TenCombo, x.GiaCombo, x.HinhAnh, x.TrangThaiHoatDong, x.ChiTiets.Count,
                x.ApDungKhungGio,
                x.GioBatDau.HasValue ? x.GioBatDau.Value.ToString(@"hh\:mm") : null,
                x.GioKetThuc.HasValue ? x.GioKetThuc.Value.ToString(@"hh\:mm") : null
            ))
            .ToListAsync();
    }

    public async Task<ComboDetail?> LayChiTietAsync(int id)
    {
        var c = await _db.Combos.Include(x => x.ChiTiets).ThenInclude(ct => ct.SanPham)
            .FirstOrDefaultAsync(x => x.MaCombo == id);
        if (c is null) return null;
        return new ComboDetail(
            c.MaCombo, c.TenCombo, c.GiaCombo, c.HinhAnh, c.MoTa, c.TrangThaiHoatDong,
            c.ChiTiets.Select(ct => new ComboLineDto(ct.MaSanPham, ct.SoLuong, ct.SanPham.TenSanPham)),
            c.ApDungKhungGio,
            c.GioBatDau.HasValue ? c.GioBatDau.Value.ToString(@"hh\:mm") : null,
            c.GioKetThuc.HasValue ? c.GioKetThuc.Value.ToString(@"hh\:mm") : null
        );
    }

    public async Task<ServiceResult<int>> TaoAsync(SaveComboRequest req)
    {
        var err = await KiemTraSanPham(req.ChiTiets);
        if (err is not null) return ServiceResult<int>.Fail(err);

        var c = new Combo
        {
            TenCombo = req.TenCombo.Trim(),
            GiaCombo = req.GiaCombo,
            HinhAnh = req.HinhAnh,
            MoTa = req.MoTa,
            TrangThaiHoatDong = req.TrangThaiHoatDong,
            ApDungKhungGio = req.ApDungKhungGio,
            GioBatDau = ParseTime(req.GioBatDau),
            GioKetThuc = ParseTime(req.GioKetThuc),
            ChiTiets = req.ChiTiets.Select(l => new ChiTietCombo { MaSanPham = l.MaSanPham, SoLuong = l.SoLuong }).ToList(),
        };
        _db.Combos.Add(c);
        await _db.SaveChangesAsync();
        return ServiceResult<int>.Ok(c.MaCombo);
    }

    public async Task<ServiceResult<bool>> CapNhatAsync(int id, SaveComboRequest req)
    {
        var c = await _db.Combos.Include(x => x.ChiTiets).FirstOrDefaultAsync(x => x.MaCombo == id);
        if (c is null) return ServiceResult<bool>.Fail("Không tìm thấy combo.");
        var err = await KiemTraSanPham(req.ChiTiets);
        if (err is not null) return ServiceResult<bool>.Fail(err);

        c.TenCombo = req.TenCombo.Trim();
        c.GiaCombo = req.GiaCombo;
        c.HinhAnh = req.HinhAnh;
        c.MoTa = req.MoTa;
        c.TrangThaiHoatDong = req.TrangThaiHoatDong;
        c.ApDungKhungGio = req.ApDungKhungGio;
        c.GioBatDau = ParseTime(req.GioBatDau);
        c.GioKetThuc = ParseTime(req.GioKetThuc);

        _db.ChiTietCombos.RemoveRange(c.ChiTiets);
        c.ChiTiets = req.ChiTiets.Select(l => new ChiTietCombo { MaSanPham = l.MaSanPham, SoLuong = l.SoLuong }).ToList();

        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private static TimeSpan? ParseTime(string? input) =>
        TimeSpan.TryParse(input, out var ts) ? ts : null;

    public async Task<ServiceResult<bool>> XoaAsync(int id)
    {
        var c = await _db.Combos.FindAsync(id);
        if (c is null) return ServiceResult<bool>.Fail("Không tìm thấy combo.");
        try 
        {
            _db.Combos.Remove(c);
            await _db.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            return ServiceResult<bool>.Fail("Không thể xoá vì đã có khách hàng đặt combo này. Vui lòng bấm 'Tắt' thay vì xoá để giữ nguyên lịch sử doanh thu.");
        }
    }

    private async Task<string?> KiemTraSanPham(List<ComboLineDto> lines)
    {
        if (lines is null || lines.Count == 0) return "Combo phải có ít nhất 1 sản phẩm.";
        var ids = lines.Select(l => l.MaSanPham).Distinct().ToList();
        var coCount = await _db.SanPhams.CountAsync(x => ids.Contains(x.MaSanPham));
        return coCount == ids.Count ? null : "Có sản phẩm không tồn tại trong combo.";
    }
}
