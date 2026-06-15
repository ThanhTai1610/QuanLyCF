using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.StockTakes;

[ApiController]
[Route("api/stock-takes")]
[Authorize]
public class StockTakesController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    private readonly StockTakeService _service;

    public StockTakesController(QuanLyCFDbContext db, StockTakeService service)
    {
        _db = db; _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? trangThai)
    {
        var query = _db.PhieuKhos.Where(x => x.LoaiPhieu == "KiemKe");
        if (!string.IsNullOrWhiteSpace(trangThai))
            query = query.Where(x => x.TrangThai == trangThai);

        return Ok(await query.OrderByDescending(x => x.ThoiGianTao)
            .Select(x => new StockTakeListItem(x.MaPhieu, x.ThoiGianTao, x.TrangThai, x.ChiTiets.Count))
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _db.PhieuKhos.Include(x => x.ChiTiets).ThenInclude(c => c.NguyenLieu)
            .FirstOrDefaultAsync(x => x.MaPhieu == id && x.LoaiPhieu == "KiemKe");
        if (p is null) return NotFound();

        return Ok(new StockTakeDetail(p.MaPhieu, p.ThoiGianTao, p.TrangThai, p.GhiChu,
            p.ChiTiets.Select(c => new StockTakeLineItem(
                c.MaNguyenLieu, c.NguyenLieu.TenNguyenLieu, c.SoLuong, c.SoLuongThucTe,
                (c.SoLuongThucTe ?? c.SoLuong) - c.SoLuong, c.LyDoLech))));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(CreateStockTakeRequest req)
    {
        var r = await _service.TaoAsync(req, User.MaNhanVien());
        if (r.Error is not null) return BadRequest(new { message = r.Error });
        return CreatedAtAction(nameof(Get), new { id = r.Data }, new { MaPhieu = r.Data });
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Approve(int id)
    {
        var r = await _service.DuyetAsync(id);
        return r.Error is not null ? BadRequest(new { message = r.Error }) : Ok(new { message = "Đã duyệt và điều chỉnh tồn kho." });
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Reject(int id)
    {
        var r = await _service.TuChoiAsync(id);
        return r.Error is not null ? BadRequest(new { message = r.Error }) : Ok(new { message = "Đã từ chối phiếu kiểm kê." });
    }
}
