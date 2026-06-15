using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Catalog.Categories;

[ApiController]
[Route("api/categories")]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public CategoriesController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> List([FromQuery] bool? hienThi)
    {
        var query = _db.DanhMucs.AsQueryable();
        if (hienThi is { } h) query = query.Where(x => x.TrangThaiHoatDong == h);

        var data = await query.OrderBy(x => x.ThuTuHienThi).ThenBy(x => x.TenDanhMuc)
            .Select(x => new CategoryItem(
                x.MaDanhMuc, x.TenDanhMuc, x.MaDanhMucCha, x.HinhAnh, x.ThuTuHienThi,
                x.MoTa, x.TrangThaiHoatDong, x.SanPhams.Count))
            .ToListAsync();
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(CreateCategoryRequest req)
    {
        var dm = new DanhMuc
        {
            TenDanhMuc = req.TenDanhMuc.Trim(),
            MaDanhMucCha = req.MaDanhMucCha,
            HinhAnh = req.HinhAnh,
            ThuTuHienThi = req.ThuTuHienThi,
            MoTa = req.MoTa,
            TrangThaiHoatDong = req.TrangThaiHoatDong,
        };
        _db.DanhMucs.Add(dm);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = dm.MaDanhMuc }, new { dm.MaDanhMuc });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, UpdateCategoryRequest req)
    {
        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm is null) return NotFound();
        if (req.MaDanhMucCha == id) return BadRequest(new { message = "Danh mục không thể là cha của chính nó." });

        dm.TenDanhMuc = req.TenDanhMuc.Trim();
        dm.MaDanhMucCha = req.MaDanhMucCha;
        dm.HinhAnh = req.HinhAnh;
        dm.ThuTuHienThi = req.ThuTuHienThi;
        dm.MoTa = req.MoTa;
        dm.TrangThaiHoatDong = req.TrangThaiHoatDong;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm is null) return NotFound();
        if (await _db.SanPhams.AnyAsync(x => x.MaDanhMuc == id))
            return BadRequest(new { message = "Danh mục đang có sản phẩm, không thể xoá." });
        if (await _db.DanhMucs.AnyAsync(x => x.MaDanhMucCha == id))
            return BadRequest(new { message = "Danh mục đang có danh mục con, không thể xoá." });

        _db.DanhMucs.Remove(dm);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
