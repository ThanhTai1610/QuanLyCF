namespace BackEnd.Features.Catalog.Categories;

public record CategoryItem(
    int MaDanhMuc,
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong,
    int SoSanPham);

public record CreateCategoryRequest(
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong);

public record UpdateCategoryRequest(
    string TenDanhMuc,
    int? MaDanhMucCha,
    string? HinhAnh,
    int ThuTuHienThi,
    string? MoTa,
    bool TrangThaiHoatDong);
