using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Catalog.Combos;

[ApiController]
[Route("api/combos")]
[Authorize]
public class CombosController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public CombosController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> List() =>
        Ok(await _db.Combos.OrderBy(x => x.TenCombo)
            .Select(x => new ComboListItem(x.MaCombo, x.TenCombo, x.GiaCombo, x.HinhAnh, x.TrangThaiHoatDong, x.ChiTiets.Count))
            .ToListAsync());

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> Get(int id)
    {
        var c = await _db.Combos.Include(x => x.ChiTiets).ThenInclude(ct => ct.SanPham)
            .FirstOrDefaultAsync(x => x.MaCombo == id);
        if (c is null) return NotFound();
        return Ok(new ComboDetail(c.MaCombo, c.TenCombo, c.GiaCombo, c.HinhAnh, c.MoTa, c.TrangThaiHoatDong,
            c.ChiTiets.Select(ct => new ComboLineDto(ct.MaSanPham, ct.SoLuong, ct.SanPham.TenSanPham))));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(SaveComboRequest req)
    {
        var err = await KiemTraSanPham(req.ChiTiets);
        if (err is not null) return BadRequest(new { message = err });

        var c = new Combo
        {
            TenCombo = req.TenCombo.Trim(),
            GiaCombo = req.GiaCombo,
            HinhAnh = req.HinhAnh,
            MoTa = req.MoTa,
            TrangThaiHoatDong = req.TrangThaiHoatDong,
            ChiTiets = req.ChiTiets.Select(l => new ChiTietCombo { MaSanPham = l.MaSanPham, SoLuong = l.SoLuong }).ToList(),
        };
        _db.Combos.Add(c);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = c.MaCombo }, new { c.MaCombo });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, SaveComboRequest req)
    {
        var c = await _db.Combos.Include(x => x.ChiTiets).FirstOrDefaultAsync(x => x.MaCombo == id);
        if (c is null) return NotFound();
        var err = await KiemTraSanPham(req.ChiTiets);
        if (err is not null) return BadRequest(new { message = err });

        c.TenCombo = req.TenCombo.Trim();
        c.GiaCombo = req.GiaCombo;
        c.HinhAnh = req.HinhAnh;
        c.MoTa = req.MoTa;
        c.TrangThaiHoatDong = req.TrangThaiHoatDong;

        _db.ChiTietCombos.RemoveRange(c.ChiTiets);
        c.ChiTiets = req.ChiTiets.Select(l => new ChiTietCombo { MaSanPham = l.MaSanPham, SoLuong = l.SoLuong }).ToList();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Combos.FindAsync(id);
        if (c is null) return NotFound();
        _db.Combos.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<string?> KiemTraSanPham(List<ComboLineDto> lines)
    {
        if (lines is null || lines.Count == 0) return "Combo phải có ít nhất 1 sản phẩm.";
        var ids = lines.Select(l => l.MaSanPham).Distinct().ToList();
        var coCount = await _db.SanPhams.CountAsync(x => ids.Contains(x.MaSanPham));
        return coCount == ids.Count ? null : "Có sản phẩm không tồn tại trong combo.";
    }
}
