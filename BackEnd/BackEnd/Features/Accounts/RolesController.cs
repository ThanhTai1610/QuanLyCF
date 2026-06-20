using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Accounts;

[ApiController]
[Route("api/roles")]
[Authorize]
public class RolesController : ControllerBase
{
    // Vai trò Quản lý (toàn quyền) — không cho sửa quyền/xoá để tránh tự khoá hệ thống.
    private const int MaVaiTroQuanLy = 1;

    private readonly QuanLyCFDbContext _db;
    public RolesController(QuanLyCFDbContext db) => _db = db;

    /// <summary>Danh sách vai trò kèm quyền + số tài khoản đang dùng.</summary>
    [HttpGet]
    [Authorize(Policy = Quyens.TaiKhoanXem)]
    public async Task<IActionResult> List()
    {
        var roles = await _db.VaiTros
            .Include(v => v.VaiTroQuyens).ThenInclude(vq => vq.Quyen)
            .OrderBy(v => v.MaVaiTro)
            .ToListAsync();
        var counts = await _db.NhanViens.GroupBy(n => n.MaVaiTro)
            .Select(g => new { g.Key, C = g.Count() })
            .ToDictionaryAsync(x => x.Key, x => x.C);

        return Ok(roles.Select(v => new RoleDetail(
            v.MaVaiTro, v.TenVaiTro, v.MoTa, v.LaVaiTroHeThong,
            counts.GetValueOrDefault(v.MaVaiTro),
            v.VaiTroQuyens.Select(q => q.Quyen.MaCode).ToList())));
    }

    /// <summary>Tất cả quyền (trang/chức năng) — để hiển thị tick khi cấu hình vai trò.</summary>
    [HttpGet("permissions")]
    [Authorize(Policy = Quyens.TaiKhoanXem)]
    public async Task<IActionResult> Permissions() =>
        Ok(await _db.Quyens.OrderBy(q => q.Nhom).ThenBy(q => q.MaQuyen)
            .Select(q => new PermissionItem(q.MaQuyen, q.MaCode, q.TenQuyen, q.Nhom))
            .ToListAsync());

    [HttpPost]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Create(SaveRoleRequest req)
    {
        var ten = req.TenVaiTro?.Trim() ?? "";
        if (ten.Length == 0) return BadRequest(new { message = "Vui lòng nhập tên vai trò." });
        if (await _db.VaiTros.AnyAsync(v => v.TenVaiTro == ten))
            return Conflict(new { message = "Tên vai trò đã tồn tại." });

        var vt = new VaiTro { TenVaiTro = ten, MoTa = req.MoTa, LaVaiTroHeThong = false };
        vt.VaiTroQuyens = await QuyenTuMaCodeAsync(req.Quyens, vt);
        _db.VaiTros.Add(vt);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = vt.MaVaiTro }, new { vt.MaVaiTro });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Update(int id, SaveRoleRequest req)
    {
        if (id == MaVaiTroQuanLy)
            return BadRequest(new { message = "Không thể sửa quyền của vai trò Quản lý (toàn quyền)." });

        var vt = await _db.VaiTros.Include(v => v.VaiTroQuyens).FirstOrDefaultAsync(v => v.MaVaiTro == id);
        if (vt is null) return NotFound();

        var ten = req.TenVaiTro?.Trim() ?? "";
        if (ten.Length == 0) return BadRequest(new { message = "Vui lòng nhập tên vai trò." });
        if (await _db.VaiTros.AnyAsync(v => v.TenVaiTro == ten && v.MaVaiTro != id))
            return Conflict(new { message = "Tên vai trò đã tồn tại." });

        if (!vt.LaVaiTroHeThong) vt.TenVaiTro = ten;   // vai trò hệ thống giữ nguyên tên
        vt.MoTa = req.MoTa;
        _db.VaiTroQuyens.RemoveRange(vt.VaiTroQuyens);
        vt.VaiTroQuyens = await QuyenTuMaCodeAsync(req.Quyens, vt);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.TaiKhoanQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var vt = await _db.VaiTros.FindAsync(id);
        if (vt is null) return NotFound();
        if (vt.LaVaiTroHeThong) return BadRequest(new { message = "Không thể xoá vai trò hệ thống." });
        if (await _db.NhanViens.AnyAsync(n => n.MaVaiTro == id))
            return BadRequest(new { message = "Vai trò đang có tài khoản, hãy chuyển tài khoản sang vai trò khác trước." });

        _db.VaiTros.Remove(vt);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private async Task<List<VaiTroQuyen>> QuyenTuMaCodeAsync(List<string>? maCodes, VaiTro vt)
    {
        if (maCodes is null || maCodes.Count == 0) return new();
        var ids = await _db.Quyens.Where(q => maCodes.Contains(q.MaCode)).Select(q => q.MaQuyen).ToListAsync();
        return ids.Select(qid => new VaiTroQuyen { MaQuyen = qid, VaiTro = vt }).ToList();
    }
}