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
    public async Task<IActionResult> Menu([FromQuery] bool isPos = false) => Ok(await _svc.LayMenuAsync(isPos));

    [HttpGet("active")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> Active() => Ok(await _svc.LayDonActiveAsync());

    [HttpGet("kitchen-active")]
    [AllowAnonymous]
    public async Task<IActionResult> KitchenActive() => Ok(await _svc.LayDonBepActiveAsync());

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _svc.LayTatCaDonHangAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _svc.LayDonTheoIdAsync(id);
        return data == null ? NotFound(new { message = "Không tìm thấy đơn hàng." }) : Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Create(CreateOrderRequest req)
    {
        var (data, err) = await _svc.TaoDonAsync(req, CurrentUserId);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }

    [HttpPost("guest")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateGuest(CreateOrderRequest req)
    {
        if (req.MaBan == null)
        {
            return BadRequest(new { message = "Khách hàng phải chọn bàn để gọi món." });
        }
        var (data, err) = await _svc.TaoDonAsync(req, null);
        if (err != null) return BadRequest(new { message = err });

        var ban = await _db.Bans.FindAsync(req.MaBan.Value);
        return Ok(new
        {
            order = data,
            maPinSession = ban?.MaPinSession
        });
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
    [AllowAnonymous]
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

    /// <summary>Lịch sử đơn hàng của 1 khách hàng (theo email đăng nhập Google/Tích điểm).</summary>
    [HttpGet("customer-history")]
    [AllowAnonymous]
    public async Task<IActionResult> CustomerHistory([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return BadRequest(new { message = "Email không được để trống." });
        var cleanEmail = email.Trim().ToLower();
        var customer = await _db.KhachHangs.FirstOrDefaultAsync(k => k.Email != null && k.Email.ToLower() == cleanEmail);
        if (customer == null) return Ok(new List<OrderDto>());

        var orders = await _db.DonHangs
            .Include(d => d.Ban)
            .Include(d => d.ChiTiets)
                .ThenInclude(c => c.SanPham)
            .Include(d => d.ChiTiets)
                .ThenInclude(c => c.KichCo)
            .Where(d => d.MaKhachHang == customer.MaKhachHang || (d.GhiChuDonHang != null && customer.SoDienThoai != null && d.GhiChuDonHang.Contains(customer.SoDienThoai)))
            .OrderByDescending(d => d.ThoiGianTao)
            .Take(30)
            .ToListAsync();

        var dtos = orders.Select(o => new OrderDto(
            o.MaDonHang,
            o.MaBan,
            o.Ban?.TenBan,
            o.LoaiDonHang,
            o.TrangThaiDon,
            o.ThanhTien,
            o.ChiTiets.Sum(c => c.SoLuong),
            o.ThoiGianTao,
            o.ChiTiets.Select(c => new OrderItemDto(
                c.MaChiTiet,
                c.MaSanPham,
                c.SanPham?.TenSanPham ?? "Món",
                c.KichCo?.TenKichCo,
                c.SoLuong,
                c.DonGia,
                c.ThanhTien,
                c.GhiChuMon,
                c.TrangThaiBep
            )).ToList()
        )).ToList();

        return Ok(dtos);
    }

    /// <summary>Khôi phục 1 đơn (đã hoàn tất/huỷ) về hoạt động.</summary>
    [HttpPost("{id:int}/restore")]
    [Authorize(Policy = Quyens.DonHangXuLy)]
    public async Task<IActionResult> Restore(int id)
    {
        var (ok, err) = await _svc.KhoiPhucDonAsync(id);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    /// <summary>Gửi mã PIN Bàn & Hóa đơn tự động qua Gmail cho khách hàng.</summary>
    [HttpPost("send-email-receipt")]
    [AllowAnonymous]
    public async Task<IActionResult> SendEmailReceipt([FromBody] SendOrderReceiptEmailRequest req, [FromServices] Shared.EmailService emailSvc)
    {
        if (string.IsNullOrWhiteSpace(req.Email))
        {
            return BadRequest(new { message = "Vui lòng nhập Email để nhận Mã PIN & Hóa đơn." });
        }

        var cleanEmail = req.Email.Trim().ToLower();
        var pinCode = req.MaPinSession ?? "---";
        var tableName = string.IsNullOrWhiteSpace(req.TenBan) ? "Đơn hàng" : req.TenBan;

        string bodyHtml = $@"
        <div style='font-family: Arial, sans-serif; max-width: 550px; margin: 0 auto; border: 1px solid #EAE3D9; border-radius: 16px; overflow: hidden; background: #FFFFFF;'>
          <div style='background: #1A1512; color: #FFFFFF; padding: 24px; text-align: center;'>
            <h1 style='color: #E89E53; margin: 0; font-size: 24px;'>F6 COFFEE</h1>
            <p style='margin: 6px 0 0 0; font-size: 13px; color: #A09890;'>Thông tin Đặt Món & Mã PIN Bàn</p>
          </div>

          <div style='padding: 24px;'>
            <p style='font-size: 14px; color: #2A231E;'>Xin chào quý khách,</p>
            <p style='font-size: 14px; color: #5C544E; line-height: 1.6;'>
              Cảm ơn quý khách đã đặt món thành công tại <strong>{tableName}</strong>! Dưới đây là thông tin mã PIN để người ngồi cùng bàn có thể tiếp tục quét QR và đặt món:
            </p>

            <div style='background: #FAF6F0; border: 2px dashed #CC8033; border-radius: 12px; padding: 18px; text-align: center; margin: 20px 0;'>
              <span style='font-size: 11px; font-weight: bold; color: #8A8178; text-transform: uppercase; letter-spacing: 1px;'>🔑 MÃ PIN BÀN CỦA BẠN:</span>
              <div style='font-size: 36px; font-weight: 900; color: #CC8033; letter-spacing: 8px; margin-top: 8px;'>{pinCode}</div>
              <p style='font-size: 11px; color: #8A8178; margin-top: 8px;'>Người đi cùng quét QR trên bàn và nhập mã 4 số này để gọi thêm món.</p>
            </div>

            <div style='border-top: 1px solid #EAE3D9; padding-top: 16px; margin-top: 20px; font-size: 12px; color: #8A8178; text-align: center;'>
              <p style='margin: 0;'>F6 Coffee chúc quý khách một buổi thưởng thức cà phê thật tuyệt vời! ☕</p>
            </div>
          </div>
        </div>";

        try
        {
            bool sent = await emailSvc.SendEmailAsync(cleanEmail, $"[F6 Coffee] Mã PIN Bàn {tableName}: {pinCode}", bodyHtml);
            if (sent)
            {
                return Ok(new { message = $"Đã gửi mã PIN bàn ({pinCode}) và thông tin hóa đơn tới email {cleanEmail} thành công!" });
            }
            return BadRequest(new { message = "Không thể gửi Email. Vui lòng kiểm tra lại địa chỉ Gmail." });
        }
        catch (Exception ex)
        {
            return Ok(new { message = $"[MÔ PHỎNG] Đã ghi nhận mã PIN {pinCode} gửi tới Email {cleanEmail}! (Cần điền SmtpPass trong appsettings.json để gửi thật)", isMock = true });
        }
    }

    /// <summary>Cập nhật trạng thái bếp của từng món (ChoLam -> HoanThanh / ChoLam).</summary>
    [HttpPut("items/{maChiTiet:int}/kitchen-status")]
    [AllowAnonymous]
    public async Task<IActionResult> UpdateItemKitchenStatus(int maChiTiet, [FromBody] UpdateKitchenStatusRequest req)
    {
        var ct = await _db.ChiTietDonHangs.FindAsync(maChiTiet);
        if (ct == null) return NotFound(new { message = "Không tìm thấy chi tiết món." });

        ct.TrangThaiBep = req.TrangThaiBep;
        if (req.TrangThaiBep == "HoanThanh") ct.ThoiGianLamXong = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return Ok(new { success = true, trangThaiBep = ct.TrangThaiBep });
    }
}

public record SendOrderReceiptEmailRequest(
    string Email,
    int? MaDonHang,
    string? TenBan,
    string? MaPinSession
);

public record UpdateKitchenStatusRequest(string TrangThaiBep);
