using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Catalog.Combos;

[ApiController]
[Route("api/combos")]
[Authorize]
public class CombosController : ControllerBase
{
    private readonly ComboService _service;

    public CombosController(ComboService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> List()
    {
        return Ok(await _service.LayDanhSachAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamXem)]
    public async Task<IActionResult> Get(int id)
    {
        var c = await _service.LayChiTietAsync(id);
        if (c is null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(SaveComboRequest req)
    {
        var r = await _service.TaoAsync(req);
        if (r.Error is not null) return BadRequest(new { message = r.Error });
        return CreatedAtAction(nameof(Get), new { id = r.Data }, new { MaCombo = r.Data });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, SaveComboRequest req)
    {
        var r = await _service.CapNhatAsync(id, req);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var r = await _service.XoaAsync(id);
        if (r.Error is not null) return r.Error.Contains("Không tìm thấy") ? NotFound() : BadRequest(new { message = r.Error });
        return NoContent();
    }
}
