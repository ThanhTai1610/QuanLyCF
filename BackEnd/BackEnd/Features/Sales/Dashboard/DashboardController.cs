using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Dashboard;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly DashboardService _service;

    public DashboardController(DashboardService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var data = await _service.GetDashboardDataAsync();
        return Ok(data);
    }

    /// <summary>GET /api/dashboard/revenue-report?year=2026 hoặc ?year=2026&month=6</summary>
    [HttpGet("revenue-report")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] int? year, [FromQuery] int? month)
    {
        int y = year ?? DateTime.UtcNow.Year;
        if (month.HasValue && (month < 1 || month > 12))
            return BadRequest("Tháng phải từ 1 đến 12.");
        var data = await _service.GetMonthlyReportAsync(y, month);
        return Ok(data);
    }
}
