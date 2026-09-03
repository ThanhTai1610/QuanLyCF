namespace BackEnd.Features.Catalog.Products;

public record SizeDto(string TenKichCo, decimal GiaCongThem, bool TrangThaiHoatDong = true);

public record ProductListItem(
    int MaSanPham,
    string TenSanPham,
    int? MaDanhMuc,
    string? TenDanhMuc,
    decimal GiaBan,
    decimal? GiaVonDuKien,
    string? HinhAnh,
    string KieuMon,
    bool LaMonNoiBat,
    bool TrangThaiBan,
    int? DiemTichLuy = 0);

public record ProductDetail(
    int MaSanPham,
    string TenSanPham,
    int? MaDanhMuc,
    string? MaVach_SKU,
    decimal GiaBan,
    decimal? GiaVonDuKien,
    string? HinhAnh,
    string? MoTa,
    int? LuongCalo,
    int? ThoiGianChuanBiPhut,
    bool LaMonNoiBat,
    string KieuMon,
    bool TrangThaiBan,
    IEnumerable<SizeDto> KichCos,
    int? DiemTichLuy = 0);

public record CreateProductRequest(
    string TenSanPham,
    int? MaDanhMuc,
    string? MaVach_SKU,
    decimal GiaBan,
    decimal? GiaVonDuKien,
    string? HinhAnh,
    string? MoTa,
    int? LuongCalo,
    int? ThoiGianChuanBiPhut,
    bool LaMonNoiBat,
    string KieuMon,
    bool TrangThaiBan,
    List<SizeDto>? KichCos,
    int? DiemTichLuy = 0);

public record UpdateProductRequest(
    string TenSanPham,
    int? MaDanhMuc,
    string? MaVach_SKU,
    decimal GiaBan,
    decimal? GiaVonDuKien,
    string? HinhAnh,
    string? MoTa,
    int? LuongCalo,
    int? ThoiGianChuanBiPhut,
    bool LaMonNoiBat,
    string KieuMon,
    bool TrangThaiBan,
    List<SizeDto>? KichCos,
    int? DiemTichLuy = 0);
