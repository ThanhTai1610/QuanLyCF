namespace BackEnd.Features.Auth;

public class JwtSettings
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = "QuanLyCF";
    public string Audience { get; set; } = "QuanLyCF";
    public int AccessTokenMinutes { get; set; } = 60;
    public int RefreshTokenDays { get; set; } = 7;
}
