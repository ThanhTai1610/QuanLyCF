using BackEnd.Features.Inventory.StockReceipts; // For ServiceResult
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Inventory.Materials;

[ApiController]
[Route("api/materials")]
[Authorize]
public class MaterialsController : ControllerBase
{
    private readonly MaterialService _service;

    public MaterialsController(MaterialService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] string? typeFilter, [FromQuery] string? statusFilter)
    {
        var data = await _service.LayDanhSachAsync(q, typeFilter, statusFilter);
        return Ok(data);
    }

    [HttpGet("summary")]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> Summary()
    {
        var data = await _service.TinhThongKeAsync();
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(SaveMaterialRequest req)
    {
        var r = await _service.TaoAsync(req);
        if (r.Error is not null) return Conflict(new { message = r.Error });
        return CreatedAtAction(nameof(List), new { id = r.Data!.MaNguyenLieu }, r.Data);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Update(int id, SaveMaterialRequest req)
    {
        var r = await _service.CapNhatAsync(id, req);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : Conflict(new { message = r.Error });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.XoaAsync(id);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }

    [HttpPost("{id:int}/adjust")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> AdjustStock(int id, AdjustStockRequest req)
    {
        var r = await _service.DieuChinhNhanhAsync(id, req.SoLuongThucTe, req.LyDo, User.MaNhanVien());
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }
}
