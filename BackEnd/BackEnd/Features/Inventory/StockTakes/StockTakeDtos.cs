namespace BackEnd.Features.Inventory.StockTakes;

public record StockTakeLineRequest(int MaNguyenLieu, decimal SoLuongThucTe, string? LyDoLech);

public record CreateStockTakeRequest(string? GhiChu, List<StockTakeLineRequest> ChiTiets);

public record StockTakeListItem(
    int MaPhieu,
    DateTime ThoiGianTao,
    string TrangThai,
    int SoMatHang);

public record StockTakeLineItem(
    int MaNguyenLieu,
    string TenNguyenLieu,
    decimal TonHeThong,
    decimal? TonThucTe,
    decimal ChenhLech,
    string? LyDoLech);

public record StockTakeDetail(
    int MaPhieu,
    DateTime ThoiGianTao,
    string TrangThai,
    string? GhiChu,
    IEnumerable<StockTakeLineItem> ChiTiets);
