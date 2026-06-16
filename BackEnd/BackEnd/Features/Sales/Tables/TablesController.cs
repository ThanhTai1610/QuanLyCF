using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Tables;

[ApiController]
[Route("api/tables")]
[Authorize]
public class TablesController : ControllerBase
{
    private static readonly string[] TrangThaiHopLe = { "Trong", "CoKhach", "BaoTri" };

    private readonly QuanLyCFDbContext _db;
    private readonly IConfiguration _cfg;
    public TablesController(QuanLyCFDbContext db, IConfiguration cfg) { _db = db; _cfg = cfg; }

    private string FeOrigin => _cfg["Cors:FrontendOrigin"]?.TrimEnd('/') ?? "http://localhost:5173";
    private static string TaoQRHash() => Guid.NewGuid().ToString("N");

    [HttpGet]
    [Authorize(Policy = Quyens.BanXem)]
    public async Task<IActionResult> List([FromQuery] int? maKhuVuc, [FromQuery] string? trangThai)
    {
        var query = _db.Bans.Include(x => x.KhuVuc).Include(x => x.BanChinh).AsQueryable();
        if (maKhuVuc is { } k) query = query.Where(x => x.MaKhuVuc == k);
        if (!string.IsNullOrWhiteSpace(trangThai)) query = query.Where(x => x.TrangThai == trangThai);

        var data = await query.OrderBy(x => x.TenBan).ToListAsync();
        return Ok(data.Select(Map));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Create(SaveTableRequest req)
    {
        if (await _db.Bans.AnyAsync(x => x.TenBan == req.TenBan.Trim()))
            return Conflict(new { message = "Tên bàn đã tồn tại." });
        if (!await _db.KhuVucBans.AnyAsync(x => x.MaKhuVuc == req.MaKhuVuc))
            return BadRequest(new { message = "Khu vực không tồn tại." });

        var ban = new Ban
        {
            TenBan = req.TenBan.Trim(),
            MaKhuVuc = req.MaKhuVuc,
            SucChua = req.SucChua,
            MaQRHash = TaoQRHash(),
            TrangThai = "Trong",
        };
        _db.Bans.Add(ban);
        await _db.SaveChangesAsync();
        await _db.Entry(ban).Reference(x => x.KhuVuc).LoadAsync();
        return CreatedAtAction(nameof(List), new { id = ban.MaBan }, Map(ban));
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Update(int id, SaveTableRequest req)
    {
        var ban = await _db.Bans.FindAsync(id);
        if (ban is null) return NotFound();
        if (await _db.Bans.AnyAsync(x => x.TenBan == req.TenBan.Trim() && x.MaBan != id))
            return Conflict(new { message = "Tên bàn đã tồn tại." });
        if (!await _db.KhuVucBans.AnyAsync(x => x.MaKhuVuc == req.MaKhuVuc))
            return BadRequest(new { message = "Khu vực không tồn tại." });

        ban.TenBan = req.TenBan.Trim();
        ban.MaKhuVuc = req.MaKhuVuc;
        ban.SucChua = req.SucChua;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Đổi trạng thái bàn: Trong / CoKhach / BaoTri.</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Policy = Quyens.BanXem)]
    public async Task<IActionResult> UpdateStatus(int id, UpdateTableStatusRequest req)
    {
        if (!TrangThaiHopLe.Contains(req.TrangThai))
            return BadRequest(new { message = "Trạng thái không hợp lệ (Trong/CoKhach/BaoTri)." });
        var ban = await _db.Bans.FindAsync(id);
        if (ban is null) return NotFound();
        ban.TrangThai = req.TrangThai;
        await _db.SaveChangesAsync();
        return Ok(new { ban.TrangThai });
    }

    /// <summary>Tạo lại mã QR (khi nghi lộ/đổi bàn).</summary>
    [HttpPost("{id:int}/regenerate-qr")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> RegenerateQr(int id)
    {
        var ban = await _db.Bans.FindAsync(id);
        if (ban is null) return NotFound();
        ban.MaQRHash = TaoQRHash();
        await _db.SaveChangesAsync();
        return Ok(new { ban.MaQRHash, UrlDatMon = $"{FeOrigin}/menu/{ban.MaQRHash}" });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var ban = await _db.Bans.FindAsync(id);
        if (ban is null) return NotFound();
        // Gỡ các bàn thành viên đang ghép vào bàn này trước khi xoá
        var thanhVien = await _db.Bans.Where(x => x.MaBanChinh == id).ToListAsync();
        foreach (var tv in thanhVien) tv.MaBanChinh = null;
        _db.Bans.Remove(ban);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Ghép nhiều bàn về 1 bàn chính (phục vụ chung 1 đoàn khách).</summary>
    [HttpPost("merge")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Merge(MergeTablesRequest req)
    {
        var chinh = await _db.Bans.FindAsync(req.MaBanChinh);
        if (chinh is null) return NotFound(new { message = "Bàn chính không tồn tại." });

        var idThanhVien = (req.MaThanhVien ?? Array.Empty<int>())
            .Where(x => x != req.MaBanChinh).Distinct().ToList();
        if (idThanhVien.Count == 0)
            return BadRequest(new { message = "Chọn ít nhất 2 bàn để ghép." });

        var thanhVien = await _db.Bans.Where(x => idThanhVien.Contains(x.MaBan)).ToListAsync();
        if (thanhVien.Count != idThanhVien.Count)
            return BadRequest(new { message = "Có bàn không tồn tại." });

        // Bàn chính trở thành bàn chính thực sự
        chinh.MaBanChinh = null;
        // Gộp thành viên về bàn chính; làm phẳng nếu thành viên từng là bàn chính của nhóm khác
        foreach (var tv in thanhVien) tv.MaBanChinh = req.MaBanChinh;
        var nhomCon = await _db.Bans.Where(x => x.MaBanChinh != null && idThanhVien.Contains(x.MaBanChinh.Value)).ToListAsync();
        foreach (var c in nhomCon) c.MaBanChinh = req.MaBanChinh;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Tách bàn: nếu là bàn chính → giải tán cả nhóm; nếu là thành viên → tách riêng bàn đó.</summary>
    [HttpPost("{id:int}/unmerge")]
    [Authorize(Policy = Quyens.BanQuanLy)]
    public async Task<IActionResult> Unmerge(int id)
    {
        var ban = await _db.Bans.FindAsync(id);
        if (ban is null) return NotFound();

        var thanhVien = await _db.Bans.Where(x => x.MaBanChinh == id).ToListAsync();
        if (thanhVien.Count > 0)
            foreach (var tv in thanhVien) tv.MaBanChinh = null;   // là bàn chính → giải tán
        else if (ban.MaBanChinh != null)
            ban.MaBanChinh = null;                                // là thành viên → tách riêng

        await _db.SaveChangesAsync();
        return NoContent();
    }

    private TableItem Map(Ban x) => new(
        x.MaBan, x.TenBan, x.MaKhuVuc, x.KhuVuc?.TenKhuVuc ?? "", x.SucChua, x.TrangThai,
        x.MaQRHash, $"{FeOrigin}/menu/{x.MaQRHash}", x.MaBanChinh, x.BanChinh?.TenBan);
}
