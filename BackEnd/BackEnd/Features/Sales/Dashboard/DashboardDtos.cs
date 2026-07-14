namespace BackEnd.Features.Sales.Dashboard;

public record DashboardStatsDto(
    decimal TodayRevenue, decimal RevenueDelta,
    int TodayOrders, int OrdersDelta,
    int Customers, decimal CustomersDelta,
    string BestItemName, int BestItemQty
);

public record DailyRevenueDto(string Day, decimal Revenue);
public record TopItemDto(string Name, int Qty);
public record RecentOrderDto(int Id, string Table, int ItemsCount, decimal Total, string Status, string CreatedAt);

public record DashboardDataDto(
    DashboardStatsDto Stats,
    List<DailyRevenueDto> RevenueData,
    List<TopItemDto> TopItems,
    List<RecentOrderDto> RecentOrders
);

// --- Báo cáo doanh thu theo tháng/năm ---
public record MonthlyRevenueDto(int Month, string MonthLabel, decimal Revenue, int Orders, decimal AvgOrder);

public record DailyRevenueDetailDto(int Day, string DayLabel, decimal Revenue, int Orders);

public record TopProductRevenueDto(string Name, int Qty, decimal Revenue);

public record MonthlyReportDto(
    int Year,
    int? Month,           // null = xem cả năm
    decimal TotalRevenue,
    int TotalOrders,
    decimal AvgOrderValue,
    decimal GrowthPercent,  // So với kỳ trước
    List<MonthlyRevenueDto> MonthlyData,      // dùng khi xem theo năm
    List<DailyRevenueDetailDto> DailyData,   // dùng khi xem theo tháng
    List<TopProductRevenueDto> TopProducts
);
