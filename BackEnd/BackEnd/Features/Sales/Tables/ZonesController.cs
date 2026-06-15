using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Tables;

[ApiController]
[Route("api/zones")]
[Authorize]
public class ZonesController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    public ZonesController(QuanLyCFDbContext db) => _db = db;

    [HttpGet]
    [Authorize(Policy = Quyens.BanXem)]
    public async Task<IActionResult> List() =>
        Ok(await _db.KhuVucBans.OrderBy(x => x.TenKhuVuc)
            .Select(x => new ZoneItem(x.MaKhuVuc, x.TenKhuVuc, x.PhuThu, x.Bans.Count))
            .ToListAsync());

    [HttpPost]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Create(SaveZoneRequest req)
    {
        var kv = new KhuVucBan { TenKhuVuc = req.TenKhuVuc.Trim(), PhuThu = req.PhuThu };
        _db.KhuVucBans.Add(kv);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(List), new { id = kv.MaKhuVuc }, new { kv.MaKhuVuc });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Update(int id, SaveZoneRequest req)
    {
        var kv = await _db.KhuVucBans.FindAsync(id);
        if (kv is null) return NotFound();
        kv.TenKhuVuc = req.TenKhuVuc.Trim();
        kv.PhuThu = req.PhuThu;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var kv = await _db.KhuVucBans.FindAsync(id);
        if (kv is null) return NotFound();
        if (await _db.Bans.AnyAsync(x => x.MaKhuVuc == id))
            return BadRequest(new { message = "Khu vực đang có bàn, không thể xoá." });
        _db.KhuVucBans.Remove(kv);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
