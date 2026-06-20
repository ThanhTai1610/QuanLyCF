using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Promotions;

[ApiController]
[Route("api/promotions")]
[Authorize]
public class PromotionsController : ControllerBase
{
    private readonly PromotionService _svc;
    public PromotionsController(PromotionService svc) => _svc = svc;

    /// <summary>Danh sách tất cả khuyến mãi (trang quản trị).</summary>
    [HttpGet]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> List() => Ok(await _svc.ListAsync());

    /// <summary>Khuyến mãi còn hiệu lực — cho POS chọn áp dụng.</summary>
    [HttpGet("active")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> Active() => Ok(await _svc.ActiveAsync());

    [HttpPost]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Create(SavePromotionRequest req)
    {
        var (id, err) = await _svc.CreateAsync(req);
        return err != null ? BadRequest(new { message = err }) : Ok(new { maKhuyenMai = id });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Update(int id, SavePromotionRequest req)
    {
        var (ok, err) = await _svc.UpdateAsync(id, req);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.SanPhamQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var (ok, err) = await _svc.DeleteAsync(id);
        return ok ? NoContent() : BadRequest(new { message = err });
    }

    /// <summary>Xem trước số tiền giảm (nhập mã hoặc chọn chương trình) — cho POS.</summary>
    [HttpPost("preview")]
    [Authorize(Policy = Quyens.DonHangXem)]
    public async Task<IActionResult> Preview(ApplyPromotionRequest req)
    {
        var (data, err) = await _svc.ApplyAsync(req);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }
}
