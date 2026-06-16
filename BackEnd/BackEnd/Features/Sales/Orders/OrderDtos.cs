namespace BackEnd.Features.Sales.Orders;

// ── Menu cho màn hình đặt món ────────────────────────────────
public record MenuSizeDto(int MaKichCo, string TenKichCo, decimal GiaCongThem);
public record MenuItemDto(
    int MaSanPham,
    string TenSanPham,
    string? TenDanhMuc,
    decimal GiaBan,
    string? HinhAnh,
    List<MenuSizeDto> KichCos);

// ── Tạo đơn ──────────────────────────────────────────────────
public record OrderLineRequest(int MaSanPham, int? MaKichCo, int SoLuong, string? GhiChuMon);
public record CreateOrderRequest(int MaBan, List<OrderLineRequest> Items, string? GhiChuDonHang);

// ── Hiển thị đơn ─────────────────────────────────────────────
public record OrderItemDto(
    int MaChiTiet,
    int? MaSanPham,
    string TenMon,
    string? TenKichCo,
    int SoLuong,
    decimal DonGia,
    decimal ThanhTien,
    string? GhiChuMon);

public record OrderDto(
    int MaDonHang,
    int? MaBan,
    string? TenBan,
    string LoaiDonHang,
    string TrangThaiDon,
    decimal ThanhTien,
    int SoMon,
    DateTime ThoiGianTao,
    List<OrderItemDto> Items);

// ── Đổi bàn / huỷ ────────────────────────────────────────────
public record MoveOrderRequest(int MaBanMoi);
public record MoveOrderResult(string KetQua, string? TenBanCu, string? TenBanMoi); // KetQua: moved | merged
public record CancelOrderRequest(string? LyDo);
