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
        try
        {
            await _service.GenerateOtpAsync(id);
            return Ok(new { 
                message = "Mã OTP đã được gửi tới địa chỉ email của khách hàng.", 
                otp = (string?)null 
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
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

    [HttpGet("public/by-email")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = "Email không được để trống." });

        var cleanEmail = email.Trim().ToLower();
        var customer = await _service.GetByEmailAsync(cleanEmail);
        if (customer == null) return NotFound(new { message = "Khách hàng không tồn tại." });

        return Ok(customer);
    }

    public record PublicRegisterBody(string Name, string Phone, string Email);

    [HttpPost("public/register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterPublic([FromBody] PublicRegisterBody body)
    {
        if (string.IsNullOrWhiteSpace(body.Name) || string.IsNullOrWhiteSpace(body.Phone) || string.IsNullOrWhiteSpace(body.Email))
            return BadRequest(new { message = "Họ tên, số điện thoại và email không được để trống." });

        var req = new CreateCustomerRequest(body.Name, body.Phone, body.Email, "Đăng ký từ trang đặt món");
        var result = await _service.CreateAsync(req);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }

        var customer = await _service.GetByIdAsync(result.Data);
        if (customer == null) return NotFound(new { message = "Khách hàng không tồn tại." });

        return Ok(new {
            id = customer.Id,
            name = customer.Name,
            phone = customer.Phone,
            email = customer.Email,
            tier = customer.Tier,
            points = customer.Points
        });
    }

    [HttpPost("public/{id:int}/send-otp")]
    [AllowAnonymous]
    public async Task<IActionResult> SendPublicOtp(int id)
    {
        try
        {
            await _service.GenerateOtpAsync(id);
            return Ok(new { message = "Mã OTP đã được gửi tới địa chỉ email của khách hàng." });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    public record VerifyOtpRequest(string Otp);

    [HttpPost("public/{id:int}/verify-otp")]
    [AllowAnonymous]
    public IActionResult VerifyPublicOtp(int id, [FromBody] VerifyOtpRequest body)
    {
        var ok = _service.VerifyOtp(id, body.Otp);
        if (!ok)
        {
            return BadRequest(new { message = "Mã OTP không chính xác hoặc đã hết hạn." });
        }
        return Ok(new { success = true });
    }

    public record RedeemPointsRequest(int Points, string Otp, int? MaDonHang);

    [HttpPost("public/{id:int}/redeem-points")]
    [AllowAnonymous]
    public async Task<IActionResult> RedeemPoints(int id, [FromBody] RedeemPointsRequest body)
    {
        var result = await _service.RedeemPointsPublicAsync(id, body.Points, body.MaDonHang);
        if (result.Error != null)
        {
            return BadRequest(new { message = result.Error });
        }
        return Ok(new { points = result.Data });
    }
}
