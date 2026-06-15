using System.Security.Claims;
using BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly QuanLyCFDbContext _db;

    public AuthController(AuthService auth, QuanLyCFDbContext db)
    {
        _auth = auth; _db = db;
    }

    /// <summary>Đăng nhập bằng email + mật khẩu (Quản lý/nhân viên).</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest req)
    {
        var r = await _auth.DangNhapAsync(req);
        return r.Data is null ? Unauthorized(new { message = r.Error }) : Ok(r.Data);
    }

    /// <summary>Đăng nhập ca làm bằng PIN (màn hình StaffLogin).</summary>
    [HttpPost("pin-login")]
    public async Task<IActionResult> PinLogin(PinLoginRequest req)
    {
        var r = await _auth.DangNhapPinAsync(req);
        return r.Data is null ? Unauthorized(new { message = r.Error }) : Ok(r.Data);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest req)
    {
        var r = await _auth.RefreshAsync(req.RefreshToken);
        return r.Data is null ? Unauthorized(new { message = r.Error }) : Ok(r.Data);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest req)
    {
        await _auth.DangXuatAsync(req.RefreshToken);
        return Ok(new { message = "Đã đăng xuất." });
    }

    /// <summary>Thông tin tài khoản đang đăng nhập.</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var id = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
                          ?? User.FindFirstValue("sub")!);
        var nv = await _db.NhanViens.Include(x => x.VaiTro).FirstOrDefaultAsync(x => x.MaNhanVien == id);
        if (nv is null) return NotFound();
        var quyens = await _auth.LayQuyenAsync(nv.MaVaiTro);
        return Ok(new AccountInfo(nv.MaNhanVien, nv.HoTen, nv.Email, nv.VaiTro?.TenVaiTro ?? "", quyens));
    }
}
