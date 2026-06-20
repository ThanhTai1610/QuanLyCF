using System.Security.Claims;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Orders;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _svc;
    public OrdersController(OrderService svc) => _svc = svc;

    private int? CurrentUserId =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
            ? id : null;

    [HttpGet("menu")]
    [Authorize(Policy = Quyens.DonHangXem)]
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
}
