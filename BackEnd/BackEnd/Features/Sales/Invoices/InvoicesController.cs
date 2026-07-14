using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Invoices;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly InvoiceService _svc;

    public InvoicesController(InvoiceService svc)
    {
        _svc = svc;
    }

    /// <summary>Lấy danh sách hoá đơn phân trang và lọc (Thu ngân / Quản lý).</summary>
    [HttpGet]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> GetList([FromQuery] InvoiceSearchQuery query)
    {
        var result = await _svc.LayDanhSachHoaDonAsync(query);
        return Ok(result);
    }

    /// <summary>Chi tiết một hoá đơn.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> GetDetail(int id)
    {
        var (data, err) = await _svc.LayChiTietHoaDonAsync(id);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        return Ok(data);
    }

    /// <summary>Sinh và xuất mã HTML in hoá đơn K80 nhiệt.</summary>
    [HttpGet("{id:int}/print")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> Print(int id)
    {
        var (html, err) = await _svc.TaoTemplateInHoaDonAsync(id);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        
        return Content(html!, "text/html", global::System.Text.Encoding.UTF8);
    }
}
