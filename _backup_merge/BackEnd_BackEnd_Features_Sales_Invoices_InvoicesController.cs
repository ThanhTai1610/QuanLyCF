using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Invoices;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _service;

    public InvoicesController(InvoiceService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.HoaDonXem)]
    public async Task<IActionResult> List([FromQuery] InvoiceQuery query)
    {
        if (query.TuNgay.HasValue && query.DenNgay.HasValue && query.TuNgay > query.DenNgay)
            return BadRequest(new { message = "Từ ngày không được sau đến ngày." });

        var result = await _service.LayDanhSachHoaDonAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.HoaDonXem)]
    public async Task<IActionResult> Get(int id)
    {
        var result = await _service.LayChiTietHoaDonAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }
}