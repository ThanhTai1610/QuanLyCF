namespace BackEnd.Features.Accounts;

public record AccountListItem(
    int MaNhanVien,
    string HoTen,
    string Email,
    int MaVaiTro,
    string TenVaiTro,
    bool TrangThaiHoatDong,
    string? SoDienThoai,
    DateTime? LanDangNhapCuoi);

public record CreateAccountRequest(
    string HoTen,
    string Email,
    int MaVaiTro,
    string MatKhau,
    string? Pin,
    string? SoDienThoai);

public record UpdateAccountRequest(
    string HoTen,
    string Email,
    int MaVaiTro,
    bool TrangThaiHoatDong,
    string? SoDienThoai);

public record ResetPasswordRequest(string MatKhauMoi);
public record SetPinRequest(string Pin);
public record RoleItem(int MaVaiTro, string TenVaiTro, string? MoTa);
