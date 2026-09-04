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
        try
        {
            var data = await _service.LayDanhSachAsync(year, month);
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CashFlow List Error] {ex.Message}");
            return Ok(new List<CashFlowListItem>());
        }
    }

    [HttpGet("summary")]
    [Authorize(Policy = Quyens.BaoCaoXem)]
    public async Task<IActionResult> Summary([FromQuery] int year, [FromQuery] int month)
    {
        if (year == 0) year = DateTime.UtcNow.Year;
        if (month == 0) month = DateTime.UtcNow.Month;
        try
        {
            var data = await _service.TinhTongKetAsync(year, month);
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CashFlow Summary Error] {ex.Message}");
            return Ok(new CashFlowSummary(0, 0, 0, 0, 0, 0));
        }
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
        try
        {
            var data = await _service.LayBangLuongAsync(year, month);
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CashFlow Salaries Error] {ex.Message}");
            return Ok(new List<SalaryListItem>());
        }
    }

    [HttpGet("test-debug")]
    [AllowAnonymous]
    public async Task<IActionResult> TestDebug([FromQuery] int year = 2026, [FromQuery] int month = 9)
    {
        try
        {
            var summary = await _service.TinhTongKetAsync(year, month);
            var list = await _service.LayDanhSachAsync(year, month);
            var salaries = await _service.LayBangLuongAsync(year, month);

            return Ok(new
            {
                success = true,
                year,
                month,
                summary,
                listCount = list.Count,
                salariesCount = salaries.Count,
                sampleItem = list.FirstOrDefault()
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                success = false,
                year,
                month,
                error = ex.ToString()
            });
        }
    }
}
