namespace BackEnd.Features.Storefront;

public record CustomerLoginRequest(string SoDienThoai, string HoTen);

public record CustomerDto(
    int MaKhachHang,
    string? HoTen,
    string? SoDienThoai,
    string HangThanhVien,
    int DiemTichLuy,
    decimal TongTienDaTieu,
    DateTime? LanGheThamCuoi,
    DateTime ThoiGianTao,
    bool IsNew
);
