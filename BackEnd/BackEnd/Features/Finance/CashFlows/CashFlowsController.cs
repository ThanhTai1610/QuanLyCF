using BackEnd.Features.Inventory.StockReceipts; // For ServiceResult
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Finance.CashFlows;

[ApiController]
[Route("api/cash-flows")]
[Authorize]
public class CashFlowsController : ControllerBase
{
    private readonly CashFlowService _service;

    public CashFlowsController(CashFlowService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.BaoCaoXem)]
    public async Task<IActionResult> List([FromQuery] int year, [FromQuery] int month)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (month == 0) month = DateTime.UtcNow.Month;
        var data = await _service.LayDanhSachAsync(year, month);
        return Ok(data);
    }

    [HttpGet("summary")]
    [Authorize(Policy = Quyens.BaoCaoXem)]
    public async Task<IActionResult> Summary([FromQuery] int year, [FromQuery] int month)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (month == 0) month = DateTime.UtcNow.Month;
        var data = await _service.TinhTongKetAsync(year, month);
        return Ok(data);
    }

    [HttpPost("out")]
    [Authorize(Policy = Quyens.CaiDatQuanLy)]
    public async Task<IActionResult> CreateCashOut(CreateCashOutRequest req)
    {
        var r = await _service.TaoPhieuChiAsync(req, User.MaNhanVien());
        if (r.Error is not null) return BadRequest(new { message = r.Error });
        return Ok(new { maDongTien = r.Data, message = "Đã tạo phiếu chi thành công." });
    }

    [HttpGet("salaries")]
    [Authorize(Policy = Quyens.BaoCaoXem)]
    public async Task<IActionResult> GetSalaries([FromQuery] int year, [FromQuery] int month)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (month == 0) month = DateTime.UtcNow.Month;
        var data = await _service.LayBangLuongAsync(year, month);
        return Ok(data);
    }
}
