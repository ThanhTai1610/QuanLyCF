namespace BackEnd.Features.Catalog.Categories;

public record CategoryItem(
    int MaDanhMuc,
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong,
    int SoSanPham,
    string LoaiDanhMuc,
    bool ApDungKhungGio,
    TimeSpan? GioBatDau,
    TimeSpan? GioKetThuc,
    int? ToiThieuChon,
    int? ToiDaChon,
    bool LaNhomKichThuoc);

public record CreateCategoryRequest(
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong,
    string LoaiDanhMuc,
    bool ApDungKhungGio,
    TimeSpan? GioBatDau,
    TimeSpan? GioKetThuc,
    int? ToiThieuChon,
    int? ToiDaChon,
    bool LaNhomKichThuoc);

public record UpdateCategoryRequest(
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong,
    string LoaiDanhMuc,
    bool ApDungKhungGio,
    TimeSpan? GioBatDau,
    TimeSpan? GioKetThuc,
    int? ToiThieuChon,
    int? ToiDaChon,
    bool LaNhomKichThuoc);

public record AssignProductsRequest(
    List<int> ProductIds);

public record CategoryOrderRequest(
    int MaDanhMuc,
    int ThuTuHienThi);