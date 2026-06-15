namespace BackEnd.Features.Auth;

public record LoginRequest(string Email, string MatKhau);
public record PinLoginRequest(int MaNhanVien, string Pin);
public record RefreshRequest(string RefreshToken);

public record TokenResponse(
    string AccessToken,
    string RefreshToken,
    DateTime HetHan,
    AccountInfo NguoiDung);

public record AccountInfo(
    int MaNhanVien,
    string HoTen,
    string Email,
    string VaiTro,
    IEnumerable<string> Quyens);
