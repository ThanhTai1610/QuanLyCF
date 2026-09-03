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
        try
        {
            var data = await _service.GetDashboardDataAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Dashboard Error] {ex.Message}");
            return Ok(new DashboardDataDto(
                new DashboardStatsDto(0, 0, 0, 0, 0, 0, "Chưa có", 0),
                new List<DailyRevenueDto>(),
                new List<TopItemDto>(),
                new List<RecentOrderDto>()
            ));
        }
    }

    /// <summary>GET /api/dashboard/revenue-report?year=2026 hoặc ?year=2026&month=6</summary>
    [HttpGet("revenue-report")]
    public async Task<IActionResult> GetRevenueReport([FromQuery] int? year, [FromQuery] int? month)
    {
        int y = year ?? DateTime.UtcNow.Year;
        if (month.HasValue && (month < 1 || month > 12))
            return BadRequest("Tháng phải từ 1 đến 12.");
        try
        {
            var data = await _service.GetMonthlyReportAsync(y, month);
            return Ok(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[RevenueReport Error] {ex.Message}");
            return Ok(new MonthlyReportDto(
                y, month, 0, 0, 0, 0,
                new List<MonthlyRevenueDto>(),
                new List<DailyRevenueDetailDto>(),
                new List<TopProductRevenueDto>()
            ));
        }
    }
}
