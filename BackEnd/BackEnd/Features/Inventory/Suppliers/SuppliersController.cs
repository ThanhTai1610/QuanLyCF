using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.Suppliers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public SuppliersController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? q)
    {
        var query = _db.NhaCungCaps.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.TenNhaCungCap.ToLower().Contains(kw)
                                  || (x.SoDienThoai != null && x.SoDienThoai.Contains(kw)));
        }
        return Ok(await query.OrderBy(x => x.TenNhaCungCap)
            .Select(x => new SupplierItem(x.MaNhaCungCap, x.TenNhaCungCap, x.NguoiLienHe, x.SoDienThoai, x.Email, x.CongNoHienTai))
            .ToListAsync());
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(SaveSupplierRequest req)
    {
        var ncc = new NhaCungCap
        {
            TenNhaCungCap = req.TenNhaCungCap.Trim(),
            MaSoThue = req.MaSoThue,
            NguoiLienHe = req.NguoiLienHe,
            SoDienThoai = req.SoDienThoai,
            Email = req.Email,
            DiaChi = req.DiaChi,
            SoTaiKhoan = req.SoTaiKhoan,
            TenNganHang = req.TenNganHang,
        };
        _db.NhaCungCaps.Add(ncc);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = ncc.MaNhaCungCap }, new { ncc.MaNhaCungCap });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Update(int id, SaveSupplierRequest req)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return NotFound();
        ncc.TenNhaCungCap = req.TenNhaCungCap.Trim();
        ncc.MaSoThue = req.MaSoThue;
        ncc.NguoiLienHe = req.NguoiLienHe;
        ncc.SoDienThoai = req.SoDienThoai;
        ncc.Email = req.Email;
        ncc.DiaChi = req.DiaChi;
        ncc.SoTaiKhoan = req.SoTaiKhoan;
        ncc.TenNganHang = req.TenNganHang;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Trả bớt công nợ cho nhà cung cấp.</summary>
    [HttpPost("{id:int}/pay")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Pay(int id, PaySupplierRequest req)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return NotFound();
        if (req.SoTien <= 0) return BadRequest(new { message = "Số tiền phải lớn hơn 0." });
        ncc.CongNoHienTai = Math.Max(0, ncc.CongNoHienTai - req.SoTien);
        await _db.SaveChangesAsync();
        return Ok(new { ncc.CongNoHienTai });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return NotFound();
        if (await _db.PhieuKhos.AnyAsync(x => x.MaNhaCungCap == id))
            return BadRequest(new { message = "Nhà cung cấp đã có phiếu nhập, không thể xoá." });
        _db.NhaCungCaps.Remove(ncc);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
