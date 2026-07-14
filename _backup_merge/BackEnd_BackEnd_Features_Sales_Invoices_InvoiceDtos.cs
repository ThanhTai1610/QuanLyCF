namespace BackEnd.Features.Sales.Invoices;

public record InvoiceListItemDto(
    int MaHoaDon,
    int MaDonHang,
    string? TenBan,
    DateTime ThoiGianThanhToan,
    decimal TongThanhTien,
    string? PhuongThuc,
    string? TenThuNgan,
    string TrangThai
);

public record InvoiceItemDto(
    string TenMon,
    int SoLuong,
    decimal DonGia,
    decimal ThanhTien
);

public record InvoiceDetailDto(
    int MaHoaDon,
    int MaDonHang,
    string? TenBan,
    DateTime ThoiGianThanhToan,
    decimal TongThanhTien,
    string? PhuongThuc,
    string? TenThuNgan,
    string TrangThai,
    decimal TongTienHang,
    decimal TienGiamGia,
    decimal PhiDichVu,
    decimal ThueVAT,
    decimal SoTienKhachTra,
    decimal TienThoiLai,
    string? MaSoThueXuatHD,
    List<InvoiceItemDto> Items
);

public record InvoiceQuery(string? Search, string? TrangThai, DateTime? TuNgay, DateTime? DenNgay);