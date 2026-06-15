namespace BackEnd.Domain.Entities;

/// <summary>Refresh token cho cơ chế JWT (cấp lại access token mà không cần đăng nhập lại).</summary>
public class RefreshToken
{
    public int MaRefreshToken { get; set; }

    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;

    public string Token { get; set; } = null!;
    public DateTime HetHan { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime? ThoiGianThuHoi { get; set; }

    public bool ConHieuLuc => ThoiGianThuHoi == null && DateTime.UtcNow < HetHan;
}
