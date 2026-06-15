using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.Materials;

[ApiController]
[Route("api/materials")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public MaterialsController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] string? trangThai)
    {
        var query = _db.NguyenLieus.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.TenNguyenLieu.ToLower().Contains(kw)
                                  || (x.MaVach_SKU != null && x.MaVach_SKU.ToLower().Contains(kw)));
        }

        var items = await query.OrderBy(x => x.TenNguyenLieu).ToListAsync();
        var data = items.Select(Map).ToList();
        if (!string.IsNullOrWhiteSpace(trangThai))
            data = data.Where(x => x.TrangThaiTon == trangThai).ToList();
        return Ok(data);
    }

    /// <summary>Thống kê: tổng SKU, sắp hết, đã rỗng.</summary>
    [HttpGet("summary")]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> Summary()
    {
        var items = await _db.NguyenLieus.ToListAsync();
        return Ok(new
        {
            tongSKU = items.Count,
            sapHet = items.Count(x => TinhTrangThai(x) == "SapHet"),
            daHet = items.Count(x => TinhTrangThai(x) == "Het"),
        });
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(SaveMaterialRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) &&
            await _db.NguyenLieus.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU))
            return Conflict(new { message = "Mã SKU đã tồn tại." });

        var nl = new NguyenLieu
        {
            TenNguyenLieu = req.TenNguyenLieu.Trim(),
            MaVach_SKU = req.MaVach_SKU,
            DonViTinh = req.DonViTinh,
            MucTonToiThieu = req.MucTonToiThieu,
            MucTonToiDa = req.MucTonToiDa,
            HanSuDungNgay = req.HanSuDungNgay,
            HinhAnh = req.HinhAnh,
            SoLuongTon = 0,
        };
        _db.NguyenLieus.Add(nl);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = nl.MaNguyenLieu }, Map(nl));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Update(int id, SaveMaterialRequest req)
    {
        var nl = await _db.NguyenLieus.FindAsync(id);
        if (nl is null) return NotFound();
        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) &&
            await _db.NguyenLieus.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU && x.MaNguyenLieu != id))
            return Conflict(new { message = "Mã SKU đã tồn tại." });

        nl.TenNguyenLieu = req.TenNguyenLieu.Trim();
        nl.MaVach_SKU = req.MaVach_SKU;
        nl.DonViTinh = req.DonViTinh;
        nl.MucTonToiThieu = req.MucTonToiThieu;
        nl.MucTonToiDa = req.MucTonToiDa;
        nl.HanSuDungNgay = req.HanSuDungNgay;
        nl.HinhAnh = req.HinhAnh;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var nl = await _db.NguyenLieus.FindAsync(id);
        if (nl is null) return NotFound();
        if (await _db.ChiTietPhieuKhos.AnyAsync(x => x.MaNguyenLieu == id))
            return BadRequest(new { message = "Nguyên liệu đã phát sinh phiếu kho, không thể xoá." });
        _db.NguyenLieus.Remove(nl);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static string TinhTrangThai(NguyenLieu x)
    {
        if (x.SoLuongTon <= 0) return "Het";
        if (x.MucTonToiThieu is { } min && x.SoLuongTon <= min) return "SapHet";
        return "Ok";
    }

    private static MaterialItem Map(NguyenLieu x) => new(
        x.MaNguyenLieu, x.TenNguyenLieu, x.MaVach_SKU, x.DonViTinh,
        x.SoLuongTon, x.MucTonToiThieu, x.GiaVonTrungBinh, TinhTrangThai(x));
}
