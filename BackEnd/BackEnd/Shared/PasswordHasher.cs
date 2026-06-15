namespace BackEnd.Shared;

/// <summary>Băm & kiểm tra mật khẩu/PIN bằng BCrypt.</summary>
public static class PasswordHasher
{
    public static string Hash(string raw) => BCrypt.Net.BCrypt.HashPassword(raw);

    public static bool Verify(string raw, string? hash)
        => !string.IsNullOrEmpty(hash) && BCrypt.Net.BCrypt.Verify(raw, hash);
}
