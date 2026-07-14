using System.Security.Claims;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Sales.Orders;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _svc;
    private readonly Infrastructure.Persistence.QuanLyCFDbContext _db;
    public OrdersController(OrderService svc, Infrastructure.Persistence.QuanLyCFDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    [HttpGet("test-seed")]
    [AllowAnonymous]
    public async Task<IActionResult> TestSeed()
    {
        var ban5 = await _db.Bans.FirstOrDefaultAsync(b => b.MaBan == 5 || b.TenBan == "Bàn 05");
        var spCaPheSua = await _db.SanPhams.Include(s => s.KichCos).FirstOrDefaultAsync(s => s.TenSanPham == "Cà phê sữa đá");
        var spBacXiu = await _db.SanPhams.Include(s => s.KichCos).FirstOrDefaultAsync(s => s.TenSanPham == "Bạc xỉu");

        if (ban5 == null || spCaPheSua == null || spBacXiu == null)
            return BadRequest("Không tìm thấy bàn hoặc sản phẩm mẫu.");

        // 1. Đơn trễ: Tạo từ 18 phút trước, đang chờ pha chế
        var donTre = new Domain.Entities.DonHang
        {
            MaBan = ban5.MaBan,
            LoaiDonHang = "DineIn",
            TrangThaiDon = "ChoXacNhan",
            TongTienHang = spCaPheSua.GiaBan,
            ThanhTien = spCaPheSua.GiaBan,
            ThoiGianTao = DateTime.UtcNow.AddMinutes(-18),
            ThoiGianCapNhat = DateTime.UtcNow.AddMinutes(-18)
        };
        donTre.ChiTiets.Add(new Domain.Entities.ChiTietDonHang
        {
            MaSanPham = spCaPheSua.MaSanPham,
            MaKichCo = spCaPheSua.KichCos.FirstOrDefault()?.MaKichCo,
            SoLuong = 1,
            DonGia = spCaPheSua.GiaBan,
            ThanhTien = spCaPheSua.GiaBan,
            TrangThaiBep = "ChoLam"
        });
        _db.DonHangs.Add(donTre);

        // 2. Đơn mới: Tạo từ 1 phút trước, đang chờ pha chế
        var donMoi = new Domain.Entities.DonHang
        {
            MaBan = ban5.MaBan,
            LoaiDonHang = "DineIn",
            TrangThaiDon = "ChoXacNhan",
            TongTienHang = spBacXiu.GiaBan,
            ThanhTien = spBacXiu.GiaBan,
            ThoiGianTao = DateTime.UtcNow.AddMinutes(-1),
            ThoiGianCapNhat = DateTime.UtcNow.AddMinutes(-1)
        };
        donMoi.ChiTiets.Add(new Domain.Entities.ChiTietDonHang
        {
            MaSanPham = spBacXiu.MaSanPham,
            MaKichCo = spBacXiu.KichCos.FirstOrDefault()?.MaKichCo,
            SoLuong = 1,
            DonGia = spBacXiu.GiaBan,
            ThanhTien = spBacXiu.GiaBan,
            TrangThaiBep = "ChoLam"
        });
        _db.DonHangs.Add(donMoi);

        await _db.SaveChangesAsync();

        return Ok(new { message = "Đã seed thành công đơn trễ (18 phút trước) và đơn mới (1 phút trước) cho Bàn 5!" });
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id : null;

    [HttpGet("menu")]
    [AllowAnonymous]
    public async Task<IActionResult> Menu() => Ok(await _svc.LayMenuAsync());

    [HttpGet("active")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> Active() => Ok(await _svc.LayDonActiveAsync());

    [HttpPost]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Create(CreateOrderRequest req)
    {
        var (data, err) = await _svc.TaoDonAsync(req, CurrentUserId);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }

    /// <summary>Tạo đơn + thanh toán (POS bán hàng tại quầy): sinh hoá đơn.</summary>
    [HttpPost("checkout")]
    [Authorize(Policy = Quyens.ThanhToan)]
    public async Task<IActionResult> Checkout(CheckoutRequest req)
    {
        var (data, err) = await _svc.ThanhToanAsync(req, CurrentUserId);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }

    /// <summary>Đổi bàn cho đơn (trống → chuyển; đã có đơn → ghép bàn giữ đơn riêng).</summary>
    [HttpPut("{id:int}/move")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Move(int id, MoveOrderRequest req)
    {
        var (data, err) = await _svc.DoiBanAsync(id, req.MaBanMoi);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }

    [HttpPut("{id:int}/cancel")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelOrderRequest? req)
    {
        var (ok, err) = await _svc.HuyDonAsync(id, req?.LyDo);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateOrderStatusRequest req)
    {
        var (ok, err) = await _svc.CapNhatTrangThaiAsync(id, req.Status);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    /// <summary>Đóng bàn: hoàn tất đơn đang hoạt động + đặt bàn Trống.</summary>
    [HttpPost("close-table/{maBan:int}")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> CloseTable(int maBan)
    {
        var (ok, err) = await _svc.DongBanAsync(maBan);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    /// <summary>Hoàn tác đóng bàn (khôi phục đơn vừa hoàn tất gần nhất).</summary>
    [HttpPost("reopen-table/{maBan:int}")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> ReopenTable(int maBan)
    {
        var (ok, err) = await _svc.MoLaiBanAsync(maBan);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    /// <summary>Lịch sử đơn của 1 bàn.</summary>
    [HttpGet("history/{maBan:int}")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> History(int maBan) => Ok(await _svc.LichSuBanAsync(maBan));

    /// <summary>Lịch sử đơn của 1 bàn dành cho khách hàng (không cần token đăng nhập).</summary>
    [HttpGet("guest/history/{maBan:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GuestHistory(int maBan) => Ok(await _svc.LichSuBanAsync(maBan));

    /// <summary>Khôi phục 1 đơn (đã hoàn tất/huỷ) về hoạt động.</summary>
    [HttpPost("{id:int}/restore")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Restore(int id)
    {
        var (ok, err) = await _svc.KhoiPhucDonAsync(id);
        return ok ? NoContent() : BadRequest(new { message = err });
    }
}
