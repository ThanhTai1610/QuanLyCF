using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Customers;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly CustomerService _service;

    public CustomerController(CustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    [Authorize(Policy = Quyens.KhachHangXem)]
    public async Task<IActionResult> List([FromQuery] string? q, [FromQuery] string? tier)
    {
        var data = await _service.ListAsync(q, tier);
        return Ok(data);
    }

    [HttpGet("{id:int}")]
    [Authorize(Policy = Quyens.KhachHangXem)]
    public async Task<IActionResult> Get(int id)
    {
        var data = await _service.GetByIdAsync(id);
        if (data == null) return NotFound(new { message = "Khách hàng không tồn tại." });
        return Ok(data);
    }

    [HttpPost]
    [Authorize(Policy = Quyens.KhachHangQuanLy)]
    public async Task<IActionResult> Create(CreateCustomerRequest req)
    {
        var result = await _service.CreateAsync(req);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }
        return CreatedAtAction(nameof(Get), new { id = result.Data }, new { id = result.Data });
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = Quyens.KhachHangQuanLy)]
    public async Task<IActionResult> Update(int id, UpdateCustomerRequest req)
    {
        var result = await _service.UpdateAsync(id, req);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = Quyens.KhachHangQuanLy)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }
        return NoContent();
    }

    [HttpGet("rewards")]
    [Authorize(Policy = Quyens.KhachHangXem)]
    public async Task<IActionResult> GetRewards()
    {
        var data = await _service.GetRewardsAsync();
        return Ok(data);
    }

    [HttpPost("{id:int}/send-otp")]
    [Authorize(Policy = Quyens.KhachHangQuanLy)]
    public async Task<IActionResult> SendOtp(int id)
    {
        var otp = await _service.GenerateOtpAsync(id);
        var isSimulated = _service.IsSmsSimulated();
        return Ok(new { 
            message = isSimulated ? "Mã OTP đã được gửi (Giả lập)." : "Mã OTP đã được gửi tới số điện thoại của khách hàng.", 
            otp = isSimulated ? otp : null 
        });
    }

    public record RedeemBody(int RewardId, string Otp);

    [HttpPost("{id:int}/redeem")]
    [Authorize(Policy = Quyens.KhachHangQuanLy)]
    public async Task<IActionResult> Redeem(int id, [FromBody] RedeemBody body)
    {
        var result = await _service.RedeemRewardAsync(id, body.RewardId, body.Otp);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }
        return Ok(new { points = result.Data });
    }
}
