namespace BackEnd.Features.Inventory.StockReceipts;

public record ReceiptLineRequest(int MaNguyenLieu, decimal SoLuong, decimal DonGia);

public record CreateReceiptRequest(
    int? MaNhaCungCap,
    decimal TienDaThanhToan,
    string? GhiChu,
    List<ReceiptLineRequest> ChiTiets);

public record ReceiptLineItem(int MaNguyenLieu, string TenNguyenLieu, decimal SoLuong, decimal DonGia, decimal ThanhTien);

public record ReceiptListItem(
    int MaPhieu,
    DateTime ThoiGianTao,
    string? TenNhaCungCap,
    decimal TongTienHang,
    decimal TienDaThanhToan,
    string TrangThaiThanhToan);

public record ReceiptDetail(
    int MaPhieu,
    DateTime ThoiGianTao,
    int? MaNhaCungCap,
    string? TenNhaCungCap,
    decimal TongTienHang,
    decimal TienDaThanhToan,
    string TrangThaiThanhToan,
    string? GhiChu,
    IEnumerable<ReceiptLineItem> ChiTiets);
