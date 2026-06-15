using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackEnd.Features.Auth;

/// <summary>Kết quả xử lý đăng nhập/refresh: thành công thì có Data, lỗi thì có Error.</summary>
public record AuthResult(TokenResponse? Data, string? Error)
{
    public static AuthResult Ok(TokenResponse t) => new(t, null);
    public static AuthResult Fail(string e) => new(null, e);
}

public class AuthService
{
    private const int MaxDangNhapSai = 5;
    private static readonly TimeSpan ThoiGianKhoa = TimeSpan.FromMinutes(15);

    private readonly QuanLyCFDbContext _db;
    private readonly JwtTokenService _jwt;
    private readonly JwtSettings _cfg;

    public AuthService(QuanLyCFDbContext db, JwtTokenService jwt, IOptions<JwtSettings> cfg)
    {
        _db = db; _jwt = jwt; _cfg = cfg.Value;
    }

    public async Task<AuthResult> DangNhapAsync(LoginRequest req)
    {
        var nv = await _db.NhanViens.Include(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.Email == req.Email);

        if (nv is null || !PasswordHasher.Verify(req.MatKhau, nv.MatKhauHash))
        {
            if (nv is not null) await GhiNhanDangNhapSaiAsync(nv);
            return AuthResult.Fail("Email hoặc mật khẩu không đúng.");
        }
        return await HoanTatDangNhapAsync(nv);
    }

    public async Task<AuthResult> DangNhapPinAsync(PinLoginRequest req)
    {
        var nv = await _db.NhanViens.Include(x => x.VaiTro)
            .FirstOrDefaultAsync(x => x.MaNhanVien == req.MaNhanVien);

        if (nv is null || !PasswordHasher.Verify(req.Pin, nv.MaPinHash))
        {
            if (nv is not null) await GhiNhanDangNhapSaiAsync(nv);
            return AuthResult.Fail("Mã PIN không đúng.");
        }
        return await HoanTatDangNhapAsync(nv);
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var rt = await _db.RefreshTokens.Include(x => x.NhanVien).ThenInclude(n => n.VaiTro)
            .FirstOrDefaultAsync(x => x.Token == refreshToken);

        if (rt is null || !rt.ConHieuLuc)
            return AuthResult.Fail("Refresh token không hợp lệ hoặc đã hết hạn.");

        rt.ThoiGianThuHoi = DateTime.UtcNow;           // xoay vòng token
        return await HoanTatDangNhapAsync(rt.NhanVien);
    }

    public async Task DangXuatAsync(string refreshToken)
    {
        var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
        if (rt is not null && rt.ThoiGianThuHoi is null)
        {
            rt.ThoiGianThuHoi = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<List<string>> LayQuyenAsync(int maVaiTro) =>
        await _db.VaiTroQuyens.Where(x => x.MaVaiTro == maVaiTro)
            .Select(x => x.Quyen.MaCode).ToListAsync();

    // ── Private ─────────────────────────────────────────────────
    private async Task<AuthResult> HoanTatDangNhapAsync(NhanVien nv)
    {
        if (!nv.TrangThaiHoatDong)
            return AuthResult.Fail("Tài khoản đã bị khoá.");
        if (nv.KhoaDenKhi is { } khoa && khoa > DateTime.UtcNow)
            return AuthResult.Fail($"Tài khoản tạm khoá đến {khoa.ToLocalTime():HH:mm} do đăng nhập sai nhiều lần.");

        var quyens = await LayQuyenAsync(nv.MaVaiTro);
        var (token, hetHan) = _jwt.TaoAccessToken(nv, quyens);

        var refresh = new RefreshToken
        {
            MaNhanVien = nv.MaNhanVien,
            Token = _jwt.TaoRefreshToken(),
            HetHan = DateTime.UtcNow.AddDays(_cfg.RefreshTokenDays),
            ThoiGianTao = DateTime.UtcNow,
        };
        _db.RefreshTokens.Add(refresh);

        nv.SoLanDangNhapSai = 0;
        nv.KhoaDenKhi = null;
        nv.LanDangNhapCuoi = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var info = new AccountInfo(nv.MaNhanVien, nv.HoTen, nv.Email, nv.VaiTro?.TenVaiTro ?? "", quyens);
        return AuthResult.Ok(new TokenResponse(token, refresh.Token, hetHan, info));
    }

    private async Task GhiNhanDangNhapSaiAsync(NhanVien nv)
    {
        nv.SoLanDangNhapSai++;
        if (nv.SoLanDangNhapSai >= MaxDangNhapSai)
        {
            nv.KhoaDenKhi = DateTime.UtcNow.Add(ThoiGianKhoa);
            nv.SoLanDangNhapSai = 0;
        }
        await _db.SaveChangesAsync();
    }
}
