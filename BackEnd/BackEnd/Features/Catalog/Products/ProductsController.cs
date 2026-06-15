using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Catalog.Products;

[ApiController]
[Route("api/products")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public ProductsController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] int? maDanhMuc, [FromQuery] bool? dangBan)
    {
        var query = _db.SanPhams.Include(x => x.DanhMuc).AsQueryable();
        if (maDanhMuc is { } c) query = query.Where(x => x.MaDanhMuc == c);
        if (dangBan is { } b) query = query.Where(x => x.TrangThaiBan == b);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.TenSanPham.ToLower().Contains(kw));
        }

        var data = await query.OrderBy(x => x.TenSanPham)
            .Select(x => new ProductListItem(
                x.MaSanPham, x.TenSanPham, x.MaDanhMuc, x.DanhMuc != null ? x.DanhMuc.TenDanhMuc : null,
                x.GiaBan, x.GiaVonDuKien, x.HinhAnh, x.KieuMon, x.LaMonNoiBat, x.TrangThaiBan))
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> Get(int id)
    {
        var x = await _db.SanPhams.Include(p => p.KichCos).FirstOrDefaultAsync(p => p.MaSanPham == id);
        if (x is null) return NotFound();
        return Ok(ToDetail(x));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(CreateProductRequest req)
    {
        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) &&
            await _db.SanPhams.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU))
            return Conflict(new { message = "Mã SKU đã tồn tại." });

        var sp = new SanPham
        {
            TenSanPham = req.TenSanPham.Trim(),
            MaDanhMuc = req.MaDanhMuc,
            MaVach_SKU = req.MaVach_SKU,
            GiaBan = req.GiaBan,
            GiaVonDuKien = req.GiaVonDuKien,
            HinhAnh = req.HinhAnh,
            MoTa = req.MoTa,
            LuongCalo = req.LuongCalo,
            ThoiGianChuanBiPhut = req.ThoiGianChuanBiPhut,
            LaMonNoiBat = req.LaMonNoiBat,
            KieuMon = string.IsNullOrWhiteSpace(req.KieuMon) ? "MonChinh" : req.KieuMon,
            TrangThaiBan = req.TrangThaiBan,
            KichCos = (req.KichCos ?? new()).Select(s => new KichCoSanPham
            {
                TenKichCo = s.TenKichCo, GiaCongThem = s.GiaCongThem, TrangThaiHoatDong = s.TrangThaiHoatDong
            }).ToList(),
        };
        _db.SanPhams.Add(sp);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = sp.MaSanPham }, ToDetail(sp));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, UpdateProductRequest req)
    {
        var sp = await _db.SanPhams.Include(x => x.KichCos).FirstOrDefaultAsync(x => x.MaSanPham == id);
        if (sp is null) return NotFound();
        if (!string.IsNullOrWhiteSpace(req.MaVach_SKU) &&
            await _db.SanPhams.AnyAsync(x => x.MaVach_SKU == req.MaVach_SKU && x.MaSanPham != id))
            return Conflict(new { message = "Mã SKU đã tồn tại." });

        sp.TenSanPham = req.TenSanPham.Trim();
        sp.MaDanhMuc = req.MaDanhMuc;
        sp.MaVach_SKU = req.MaVach_SKU;
        sp.GiaBan = req.GiaBan;
        sp.GiaVonDuKien = req.GiaVonDuKien;
        sp.HinhAnh = req.HinhAnh;
        sp.MoTa = req.MoTa;
        sp.LuongCalo = req.LuongCalo;
        sp.ThoiGianChuanBiPhut = req.ThoiGianChuanBiPhut;
        sp.LaMonNoiBat = req.LaMonNoiBat;
        sp.KieuMon = string.IsNullOrWhiteSpace(req.KieuMon) ? "MonChinh" : req.KieuMon;
        sp.TrangThaiBan = req.TrangThaiBan;

        // Thay toàn bộ danh sách size
        _db.KichCoSanPhams.RemoveRange(sp.KichCos);
        sp.KichCos = (req.KichCos ?? new()).Select(s => new KichCoSanPham
        {
            TenKichCo = s.TenKichCo, GiaCongThem = s.GiaCongThem, TrangThaiHoatDong = s.TrangThaiHoatDong
        }).ToList();

        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var sp = await _db.SanPhams.FindAsync(id);
        if (sp is null) return NotFound();
        if (await _db.ChiTietCombos.AnyAsync(x => x.MaSanPham == id))
            return BadRequest(new { message = "Sản phẩm đang nằm trong combo, không thể xoá." });
        _db.SanPhams.Remove(sp);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private static ProductDetail ToDetail(SanPham x) => new(
        x.MaSanPham, x.TenSanPham, x.MaDanhMuc, x.MaVach_SKU, x.GiaBan, x.GiaVonDuKien,
        x.HinhAnh, x.MoTa, x.LuongCalo, x.ThoiGianChuanBiPhut, x.LaMonNoiBat, x.KieuMon, x.TrangThaiBan,
        x.KichCos.Select(s => new SizeDto(s.TenKichCo, s.GiaCongThem, s.TrangThaiHoatDong)));
}
