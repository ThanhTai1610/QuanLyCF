using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Inventory.Suppliers;

[ApiController]
[Route("api/suppliers")]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly SupplierService _service;

    public SuppliersController(SupplierService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? q)
    {
        return Ok(await _service.LayDanhSachAsync(q));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(SaveSupplierRequest req)
    {
        var r = await _service.TaoAsync(req);
        if (r.Error is not null) return BadRequest(new { message = r.Error });
        return CreatedAtAction(nameof(List), new { id = r.Data }, new { MaNhaCungCap = r.Data });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Update(int id, SaveSupplierRequest req)
    {
        var r = await _service.CapNhatAsync(id, req);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }

    /// <summary>Trả bớt công nợ cho nhà cung cấp.</summary>
    [HttpPost("{id:int}/pay")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Pay(int id, PaySupplierRequest req)
    {
        var r = await _service.TraCongNoAsync(id, req.SoTien, req.PhuongThucThanhToan, User.MaNhanVien());
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return Ok(new { CongNoHienTai = r.Data });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.XoaAsync(id);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }
}
