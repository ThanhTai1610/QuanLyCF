namespace BackEnd.Features.Sales.Orders;

// ── Menu cho màn hình đặt món ────────────────────────────────
public record MenuSizeDto(int MaKichCo, string TenKichCo, decimal GiaCongThem);
public record MenuItemDto(
    int MaSanPham,
    string TenSanPham,
    string? TenDanhMuc,
    decimal GiaBan,
    string? HinhAnh,
    string KieuMon,
    string? MoTa,
    bool LaMonNoiBat,
    List<MenuSizeDto> KichCos);

// ── Tạo đơn ──────────────────────────────────────────────────
public record OrderLineRequest(int MaSanPham, int? MaKichCo, int SoLuong, string? GhiChuMon);
public record CreateOrderRequest(int? MaBan, List<OrderLineRequest> Items, string? GhiChuDonHang); // MaBan null = mang về

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

// ── Thanh toán (POS) ─────────────────────────────────────────
public record CheckoutRequest(
    int? MaBan,
    List<OrderLineRequest> Items,
    string? GhiChuDonHang,
    string PhuongThuc,          // TienMat, ChuyenKhoan, Momo
    decimal? SoTienKhachTra,
    int? MaKhuyenMai);          // khuyến mãi áp dụng (tuỳ chọn)
public record CheckoutResult(int MaDonHang, int MaHoaDon, decimal TienGiam, decimal ThanhTien, decimal TienThoiLai, string PhuongThuc);
public record MoveOrderResult(string KetQua, string? TenBanCu, string? TenBanMoi); // KetQua: moved | merged
public record CancelOrderRequest(string? LyDo);
