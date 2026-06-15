using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BackEnd.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BackEnd.Features.Auth;

public class JwtTokenService
{
    public const string PermissionClaim = "perm";
    private readonly JwtSettings _cfg;

    public JwtTokenService(IOptions<JwtSettings> cfg) => _cfg = cfg.Value;

    /// <summary>Tạo access token chứa danh tính + vai trò + danh sách quyền.</summary>
    public (string token, DateTime hetHan) TaoAccessToken(NhanVien nv, IEnumerable<string> quyens)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, nv.MaNhanVien.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.Name, nv.HoTen),
            new(ClaimTypes.Email, nv.Email),
            new(ClaimTypes.Role, nv.VaiTro?.TenVaiTro ?? string.Empty),
        };
        claims.AddRange(quyens.Select(q => new Claim(PermissionClaim, q)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var hetHan = DateTime.UtcNow.AddMinutes(_cfg.AccessTokenMinutes);

        var jwt = new JwtSecurityToken(
            issuer: _cfg.Issuer,
            audience: _cfg.Audience,
            claims: claims,
            expires: hetHan,
            signingCredentials: creds);

        return (new JwtSecurityTokenHandler().WriteToken(jwt), hetHan);
    }

    public string TaoRefreshToken() =>
        Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
}
