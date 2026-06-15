using System.Security.Claims;

namespace BackEnd.Shared;

public static class ClaimsPrincipalExtensions
{
    /// <summary>Lấy MaNhanVien từ claim "sub"/NameIdentifier của JWT.</summary>
    public static int MaNhanVien(this ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
        return int.TryParse(id, out var v) ? v : 0;
    }
}
