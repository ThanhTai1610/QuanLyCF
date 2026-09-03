using System.Security.Claims;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Payments;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _svc;

    public PaymentsController(PaymentService svc)
    {
        _svc = svc;
    }

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id : null;

    /// <summary>Thanh toán bằng Tiền mặt (Khách hàng hoặc Thu ngân).</summary>
    [HttpPost("cash")]
    [AllowAnonymous]
    public async Task<IActionResult> PayCash([FromBody] CashPaymentRequest req)
    {
        var (data, err) = await _svc.ThanhToanTienMatAsync(req.MaDonHang, req.SoTienKhachTra, CurrentUserId, req.MaKhuyenMai);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        return Ok(data);
    }

    /// <summary>Tạo link thanh toán MoMo (Khách hàng hoặc nhân viên đều có thể tạo).</summary>
    [HttpPost("momo")]
    [AllowAnonymous]
    public async Task<IActionResult> PayMomo([FromBody] MomoPaymentRequest req)
    {
        // Lấy host của backend để tạo link IPN động nếu cần
        var host = $"{Request.Scheme}://{Request.Host}";
        var (data, err) = await _svc.TaoThanhToanMomoAsync(req.MaDonHang, CurrentUserId, req.MaKhuyenMai, host);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        return Ok(data);
    }

    /// <summary>Webhook nhận kết quả thanh toán từ MoMo (IPN).</summary>
    [HttpPost("momo-ipn")]
    [AllowAnonymous]
    public async Task<IActionResult> MomoIpn([FromBody] MomoIpnRequest req)
    {
        var (success, message) = await _svc.ProcessMomoIpnAsync(req);
        if (!success)
        {
            // MoMo khuyên nếu lỗi signature thì trả về HTTP 400 để họ biết
            return BadRequest(new { message });
        }
        // Thành công trả về 204 No Content
        return NoContent();
    }

    /// <summary>Chủ động đối soát trạng thái giao dịch MoMo từ cổng MoMo.</summary>
    [HttpPost("momo-query/{maDonHang:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> QueryMomo(int maDonHang, [FromBody] MomoQueryRequest? req)
    {
        var (data, err) = await _svc.DoiSoatMomoTransactionAsync(maDonHang, req?.OrderId, req?.RequestId);
        if (err is not null)
        {
            return BadRequest(new { message = err, data });
        }
        return Ok(data);
    }

    /// <summary>Tạo mã VietQR động để chuyển khoản.</summary>
    [HttpPost("vietqr")]
    [AllowAnonymous]
    public async Task<IActionResult> PayVietQr([FromBody] MomoPaymentRequest req)
    {
        var (data, err) = await _svc.TaoThanhToanVietQrAsync(req.MaDonHang, CurrentUserId, req.MaKhuyenMai);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        return Ok(data);
    }

    /// <summary>Xác nhận chuyển khoản ngân hàng thủ công (Do Thu ngân duyệt).</summary>
    [HttpPost("confirm-transfer/{maDonHang:int}")]
    [Authorize(Policy = Quyens.ThanhToan)]
    public async Task<IActionResult> ConfirmTransfer(int maDonHang, [FromBody] ConfirmTransferRequest? req)
    {
        var (data, err) = await _svc.ConfirmChuyenKhoanThuCongAsync(maDonHang, req?.SoTienThucNhan, CurrentUserId);
        if (err is not null)
        {
            return BadRequest(new { message = err });
        }
        return Ok(data);
    }

    /// <summary>Webhook tự động nhận tiền từ Casso.</summary>
    [HttpPost("casso-webhook")]
    [AllowAnonymous]
    public async Task<IActionResult> CassoWebhook([FromBody] CassoWebhookRequest req)
    {
        var (success, message) = await _svc.ProcessCassoWebhookAsync(req);
        if (!success)
        {
            return BadRequest(new { message });
        }
        return Ok(new { success = true, message });
    }

    /// <summary>Lấy trạng thái thanh toán của đơn hàng (Trong DB của hệ thống).</summary>
    [HttpGet("status/{maDonHang:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatus(int maDonHang)
    {
        var status = await _svc.LayTrangThaiThanhToanAsync(maDonHang);
        return Ok(status);
    }
}
