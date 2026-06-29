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
