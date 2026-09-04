using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Accounts;

[ApiController]
[Route("api/accounts")]
[Authorize]
public class AccountsController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public AccountsController(QuanLyCFDbContext db) => _db = db;

    /// <summary>Danh sách tài khoản (lọc theo từ khoá + vai trò).</summary>
    [HttpGet]
    [Authorize(Policy = Quyens.TaiKhoanXem)]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] int? maVaiTro)
    {
        var query = _db.NhanViens.Include(x => x.VaiTro).AsQueryable();
        if (maVaiTro is { } r) query = query.Where(x => x.MaVaiTro == r);
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.HoTen.ToLower().Contains(kw) || x.Email.ToLower().Contains(kw));
        }

        var data = await query.OrderBy(x => x.HoTen)
            .Select(x => new AccountListItem(
                x.MaNhanVien, x.HoTen, x.Email, x.MaVaiTro, x.VaiTro.TenVaiTro,
                x.TrangThaiHoatDong, x.SoDienThoai, x.LanDangNhapCuoi))
            .ToListAsync();
        return Ok(data);
    }

    [HttpGet("roles")]
    [Authorize(Policy = Quyens.TaiKhoanXem)]
    public async Task<IActionResult> Roles() =>
        Ok(await _db.VaiTros.Select(v => new RoleItem(v.MaVaiTro, v.TenVaiTro, v.MoTa)).ToListAsync());

    [HttpPost]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Create(CreateAccountRequest req)
    {
        var cleanEmail = (req.Email ?? "").Trim().ToLower();
        var cleanName = (req.HoTen ?? "").Trim().ToLower();

        if (string.IsNullOrWhiteSpace(cleanEmail))
            return BadRequest(new { message = "Email không được để trống." });

        if (await _db.NhanViens.AnyAsync(x => x.Email.ToLower() == cleanEmail))
            return Conflict(new { message = $"Email '{req.Email}' đã được dùng cho tài khoản khác. Vui lòng nhập Email khác!" });

        if (await _db.NhanViens.AnyAsync(x => x.HoTen.ToLower() == cleanName))
            return Conflict(new { message = $"Tên nhân viên '{req.HoTen}' đã tồn tại trong hệ thống. Vui lòng nhập tên phân biệt hoặc họ tên đầy đủ!" });

        if (!await _db.VaiTros.AnyAsync(v => v.MaVaiTro == req.MaVaiTro))
            return BadRequest(new { message = "Vai trò không tồn tại." });

        var nv = new NhanVien
        {
            HoTen = req.HoTen.Trim(),
            Email = req.Email.Trim().ToLower(),
            MaVaiTro = req.MaVaiTro,
            MatKhauHash = PasswordHasher.Hash(req.MatKhau),
            MaPinHash = string.IsNullOrWhiteSpace(req.Pin) ? null : PasswordHasher.Hash(req.Pin),
            SoDienThoai = req.SoDienThoai,
        };
        _db.NhanViens.Add(nv);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = nv.MaNhanVien }, new { nv.MaNhanVien });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Update(int id, UpdateAccountRequest req)
    {
        var nv = await _db.NhanViens.FindAsync(id);
        if (nv is null) return NotFound();

        var cleanEmail = (req.Email ?? "").Trim().ToLower();
        var cleanName = (req.HoTen ?? "").Trim().ToLower();

        if (await _db.NhanViens.AnyAsync(x => x.Email.ToLower() == cleanEmail && x.MaNhanVien != id))
            return Conflict(new { message = $"Email '{req.Email}' đã được dùng cho tài khoản khác." });

        if (await _db.NhanViens.AnyAsync(x => x.HoTen.ToLower() == cleanName && x.MaNhanVien != id))
            return Conflict(new { message = $"Tên nhân viên '{req.HoTen}' đã tồn tại trong hệ thống." });

        nv.HoTen = req.HoTen.Trim();
        nv.Email = req.Email.Trim().ToLower();
        nv.MaVaiTro = req.MaVaiTro;
        nv.TrangThaiHoatDong = req.TrangThaiHoatDong;
        nv.SoDienThoai = req.SoDienThoai;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Khoá / mở khoá tài khoản.</summary>
    [HttpPost("{id:int}/toggle-lock")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> ToggleLock(int id)
    {
        var nv = await _db.NhanViens.FindAsync(id);
        if (nv is null) return NotFound();
        nv.TrangThaiHoatDong = !nv.TrangThaiHoatDong;
        if (nv.TrangThaiHoatDong) { nv.KhoaDenKhi = null; nv.SoLanDangNhapSai = 0; }
        await _db.SaveChangesAsync();
        return Ok(new { nv.TrangThaiHoatDong });
    }

    [HttpPost("{id:int}/reset-password")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> ResetPassword(int id, ResetPasswordRequest req)
    {
        var nv = await _db.NhanViens.FindAsync(id);
        if (nv is null) return NotFound(new { message = "Không tìm thấy tài khoản." });
        if (string.IsNullOrWhiteSpace(req.MatKhauMoi))
            return BadRequest(new { message = "Mật khẩu mới không được để trống." });

        var cleanPwd = req.MatKhauMoi.Trim();
        nv.MatKhauHash = PasswordHasher.Hash(cleanPwd);
        nv.SoLanDangNhapSai = 0;
        nv.KhoaDenKhi = null;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{id:int}/set-pin")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> SetPin(int id, SetPinRequest req)
    {
        var nv = await _db.NhanViens.FindAsync(id);
        if (nv is null) return NotFound(new { message = "Không tìm thấy tài khoản." });
        if (string.IsNullOrWhiteSpace(req.Pin))
            return BadRequest(new { message = "Mã PIN không được để trống." });

        var cleanPin = req.Pin.Trim();
        nv.MaPinHash = PasswordHasher.Hash(cleanPin);
        nv.MatKhauHash = PasswordHasher.Hash(cleanPin);
        nv.SoLanDangNhapSai = 0;
        nv.KhoaDenKhi = null;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var nv = await _db.NhanViens.FindAsync(id);
        if (nv is null) return NotFound();
        _db.NhanViens.Remove(nv);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
