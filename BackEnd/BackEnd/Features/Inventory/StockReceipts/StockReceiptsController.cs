using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.StockReceipts;

[ApiController]
[Route("api/stock-receipts")]
[Authorize]
public class StockReceiptsController : ControllerBase
{
    private readonly QuanLyCFDbContext _db;
    private readonly StockReceiptService _service;

    public StockReceiptsController(QuanLyCFDbContext db, StockReceiptService service)
    {
        _db = db; _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> List([FromQuery] string? trangThaiThanhToan)
    {
        var query = _db.PhieuKhos.Include(x => x.NhaCungCap)
            .Where(x => x.LoaiPhieu == "NhapKho");
        if (!string.IsNullOrWhiteSpace(trangThaiThanhToan))
            query = query.Where(x => x.TrangThaiThanhToan == trangThaiThanhToan);

        return Ok(await query.OrderByDescending(x => x.ThoiGianTao)
            .Select(x => new ReceiptListItem(
                x.MaPhieu, x.ThoiGianTao, x.NhaCungCap != null ? x.NhaCungCap.TenNhaCungCap : null,
                x.TongTienHang ?? 0, x.TienDaThanhToan ?? 0, x.TrangThaiThanhToan ?? ""))
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.KhoXem)]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _db.PhieuKhos.Include(x => x.NhaCungCap)
            .Include(x => x.ChiTiets).ThenInclude(c => c.NguyenLieu)
            .FirstOrDefaultAsync(x => x.MaPhieu == id && x.LoaiPhieu == "NhapKho");
        if (p is null) return NotFound();

        return Ok(new ReceiptDetail(
            p.MaPhieu, p.ThoiGianTao, p.MaNhaCungCap, p.NhaCungCap?.TenNhaCungCap,
            p.TongTienHang ?? 0, p.TienDaThanhToan ?? 0, p.TrangThaiThanhToan ?? "", p.GhiChu,
            p.ChiTiets.Select(c => new ReceiptLineItem(
                c.MaNguyenLieu, c.NguyenLieu.TenNguyenLieu, c.SoLuong, c.DonGia ?? 0,
                c.SoLuong * (c.DonGia ?? 0)))));
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhoQuanLy)]
    public async Task<IActionResult> Create(CreateReceiptRequest req)
    {
        var r = await _service.NhapKhoAsync(req, User.MaNhanVien());
        if (r.Error is not null) return BadRequest(new { message = r.Error });
        return CreatedAtAction(nameof(Get), new { id = r.Data }, new { MaPhieu = r.Data });
    }
}
