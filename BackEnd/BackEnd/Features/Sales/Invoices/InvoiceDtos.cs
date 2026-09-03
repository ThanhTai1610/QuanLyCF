namespace BackEnd.Features.Sales.Invoices;

public record InvoiceSearchQuery(
    int Page = 1,
    int PageSize = 10,
    DateTime? TuNgay = null,
    DateTime? DenNgay = null,
    string? TrangThai = null,     // ChuaTT, DaThanhToan
    string? PhuongThuc = null,    // TienMat, Momo, ChuyenKhoan
    int? MaDonHang = null,
    string? TenBan = null
);

public record InvoiceItemDto(
    string TenSanPham,
    string? TenKichCo,
    int SoLuong,
    decimal DonGia,
    decimal ThanhTien,
    string? GhiChuMon
);

public record InvoicePaymentDto(
    string PhuongThuc,
    decimal SoTien,
    string? MaGiaoDichCong,
    DateTime ThoiGianThanhToan
);

public record InvoiceListItemDto(
    int MaHoaDon,
    int MaDonHang,
    string? TenBan,
    string? LoaiDonHang,
    decimal TongThanhTien,
    string TrangThai,
    DateTime ThoiGianThanhToan,
    string? TenThuNgan,
    string? PhuongThuc
);

public record InvoiceDetailDto(
    int MaHoaDon,
    int MaDonHang,
    string? TenBan,
    string LoaiDonHang,
    decimal TongTienHang,
    decimal TienGiam,
    decimal ThanhTien,
    decimal SoTienKhachTra,
    decimal TienThoiLai,
    string TrangThai,
    DateTime ThoiGianThanhToan,
    string? TenNhanVienThuNgan,
    string? MaSoThueXuatHD,
    List<InvoiceItemDto> Items,
    List<InvoicePaymentDto> Payments
);
