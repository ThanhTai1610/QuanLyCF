using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Dashboard;

public class DashboardService
{
    private readonly QuanLyCFDbContext _db;

    public DashboardService(QuanLyCFDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardDataDto> GetDashboardDataAsync()
    {
        var today = DateTime.UtcNow.Date;
        var yesterday = today.AddDays(-1);
        
        // 1. Stats today vs yesterday
        var todayOrders = await _db.DonHangs
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Where(d => d.ThoiGianTao >= today && d.TrangThaiDon != "Huy")
            .ToListAsync();
            
        var yesterdayOrders = await _db.DonHangs
            .Include(d => d.ChiTiets)
            .Where(d => d.ThoiGianTao >= yesterday && d.ThoiGianTao < today && d.TrangThaiDon != "Huy")
            .ToListAsync();

        var todayRev = todayOrders.Sum(d => d.ThanhTien);
        var yesRev = yesterdayOrders.Sum(d => d.ThanhTien);
        var revDelta = yesRev > 0 ? (todayRev - yesRev) / yesRev * 100 : (todayRev > 0 ? 100 : 0);

        var todayCnt = todayOrders.Count;
        var yesCnt = yesterdayOrders.Count;
        var ordDelta = todayCnt - yesCnt;

        // Khách phục vụ (ước tính dựa trên số món / 2)
        var todayCust = todayOrders.Sum(d => d.ChiTiets.Sum(c => c.SoLuong)) / 2;
        if (todayCust == 0 && todayCnt > 0) todayCust = todayCnt; // Ít nhất 1 đơn = 1 khách
        var yesCust = yesterdayOrders.Sum(d => d.ChiTiets.Sum(c => c.SoLuong)) / 2;
        if (yesCust == 0 && yesCnt > 0) yesCust = yesCnt;
        var custDelta = yesCust > 0 ? (decimal)(todayCust - yesCust) / yesCust * 100 : (todayCust > 0 ? 100 : 0);

        // Top items today
        var itemsToday = todayOrders
            .SelectMany(d => d.ChiTiets)
            .Where(c => c.SanPham != null && c.SanPham.KieuMon != "Topping")
            .GroupBy(c => c.SanPham!.TenSanPham)
            .Select(g => new { Name = g.Key, Qty = g.Sum(c => c.SoLuong) })
            .OrderByDescending(x => x.Qty)
            .ToList();

        var bestItem = itemsToday.FirstOrDefault();
        string bestItemName = bestItem?.Name ?? "Chưa có";
        int bestItemQty = bestItem?.Qty ?? 0;

        var stats = new DashboardStatsDto(todayRev, Math.Round(revDelta, 1), todayCnt, ordDelta, todayCust, Math.Round(custDelta, 1), bestItemName, bestItemQty);
        var topItems = itemsToday.Take(5).Select(x => new TopItemDto(x.Name, x.Qty)).ToList();

        // 2. Revenue 7 days
        var sevenDaysAgo = today.AddDays(-6);
        var weekOrders = await _db.DonHangs
            .Where(d => d.ThoiGianTao >= sevenDaysAgo && d.TrangThaiDon != "Huy")
            .Select(d => new { d.ThoiGianTao, d.ThanhTien })
            .ToListAsync();

        var revData = new List<DailyRevenueDto>();
        for (int i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var sum = weekOrders.Where(d => d.ThoiGianTao.Date == date).Sum(d => d.ThanhTien);
            // Day of week VN
            var dayStr = date.DayOfWeek == DayOfWeek.Sunday ? "CN" : "T" + ((int)date.DayOfWeek + 1);
            revData.Add(new DailyRevenueDto(dayStr, sum));
        }

        // 3. Recent 7 orders
        var recent = await _db.DonHangs
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets)
            .OrderByDescending(d => d.ThoiGianTao)
            .Take(7)
            .ToListAsync();

        var recentList = recent.Select(d => new RecentOrderDto(
            d.MaDonHang,
            d.LoaiDonHang == "TakeAway" ? "Mang về" : (d.Ban?.TenBan ?? "Tại quầy"),
            d.ChiTiets.Sum(c => c.SoLuong),
            d.ThanhTien,
            d.TrangThaiDon,
            d.ThoiGianTao.AddHours(7).ToString("HH:mm")
        )).ToList();

        return new DashboardDataDto(stats, revData, topItems, recentList);
    }

    /// <summary>
    /// Báo cáo doanh thu theo tháng trong năm, hoặc theo ngày trong tháng.
    /// year: bắt buộc. month: tuỳ chọn — nếu có thì trả theo ngày trong tháng đó.
    /// </summary>
    public async Task<MonthlyReportDto> GetMonthlyReportAsync(int year, int? month)
    {
        DateTime periodStart, periodEnd, prevStart, prevEnd;

        if (month.HasValue)
        {
            // Xem theo từng ngày trong tháng
            periodStart = new DateTime(year, month.Value, 1, 0, 0, 0, DateTimeKind.Utc);
            periodEnd   = periodStart.AddMonths(1);
            prevStart   = periodStart.AddMonths(-1);
            prevEnd     = periodStart;
        }
        else
        {
            // Xem 12 tháng trong năm
            periodStart = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            periodEnd   = new DateTime(year + 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            prevStart   = new DateTime(year - 1, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            prevEnd     = periodStart;
        }

        // Lấy đơn hàng kỳ hiện tại
        var orders = await _db.DonHangs
            .Include(d => d.ChiTiets).ThenInclude(c => c.SanPham)
            .Where(d => d.ThoiGianTao >= periodStart && d.ThoiGianTao < periodEnd
                     && d.TrangThaiDon != "Huy")
            .ToListAsync();

        // Lấy đơn hàng kỳ trước để tính tăng trưởng
        var prevRevenue = await _db.DonHangs
            .Where(d => d.ThoiGianTao >= prevStart && d.ThoiGianTao < prevEnd
                     && d.TrangThaiDon != "Huy")
            .SumAsync(d => (decimal?)d.ThanhTien) ?? 0;

        var totalRevenue = orders.Sum(d => d.ThanhTien);
        var totalOrders  = orders.Count;
        var avgOrder     = totalOrders > 0 ? totalRevenue / totalOrders : 0;
        var growth       = prevRevenue > 0 ? Math.Round((totalRevenue - prevRevenue) / prevRevenue * 100, 1)
                           : (totalRevenue > 0 ? 100 : 0);

        // --- Dữ liệu biểu đồ ---
        List<MonthlyRevenueDto> monthlyData = [];
        List<DailyRevenueDetailDto> dailyData = [];

        if (month.HasValue)
        {
            // Theo ngày
            int daysInMonth = DateTime.DaysInMonth(year, month.Value);
            for (int d = 1; d <= daysInMonth; d++)
            {
                var dayOrders = orders.Where(o => o.ThoiGianTao.Day == d).ToList();
                dailyData.Add(new DailyRevenueDetailDto(
                    d,
                    $"{d:D2}/{month.Value:D2}",
                    dayOrders.Sum(o => o.ThanhTien),
                    dayOrders.Count
                ));
            }
        }
        else
        {
            // Theo tháng
            for (int m = 1; m <= 12; m++)
            {
                var mOrders = orders.Where(o => o.ThoiGianTao.Month == m).ToList();
                var mRev    = mOrders.Sum(o => o.ThanhTien);
                var mCnt    = mOrders.Count;
                monthlyData.Add(new MonthlyRevenueDto(
                    m,
                    $"T{m}",
                    mRev,
                    mCnt,
                    mCnt > 0 ? Math.Round(mRev / mCnt, 0) : 0
                ));
            }
        }

        // Top 10 sản phẩm doanh thu cao nhất
        var topProducts = orders
            .SelectMany(o => o.ChiTiets)
            .Where(c => c.SanPham != null && c.SanPham.KieuMon != "Topping")
            .GroupBy(c => c.SanPham!.TenSanPham)
            .Select(g => new TopProductRevenueDto(
                g.Key,
                g.Sum(c => c.SoLuong),
                g.Sum(c => c.ThanhTien)
            ))
            .OrderByDescending(x => x.Revenue)
            .Take(10)
            .ToList();

        return new MonthlyReportDto(
            year, month,
            Math.Round(totalRevenue, 0),
            totalOrders,
            Math.Round(avgOrder, 0),
            growth,
            monthlyData,
            dailyData,
            topProducts
        );
    }
}
