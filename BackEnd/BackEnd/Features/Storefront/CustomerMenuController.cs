using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Storefront;

[ApiController]
[Route("api/storefront/customer")]
[AllowAnonymous]
public class CustomerMenuController : ControllerBase
{
    private readonly CustomerService _svc;
    public CustomerMenuController(CustomerService svc) => _svc = svc;

    /// <summary>Đăng nhập / đăng ký khách hàng bằng SĐT + tên.</summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(CustomerLoginRequest req)
    {
        var (data, err) = await _svc.LoginAsync(req);
        return err != null ? BadRequest(new { message = err }) : Ok(data);
    }

    /// <summary>Danh sách khách hàng (admin). Có thể tìm theo tên hoặc SĐT.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var list = await _svc.GetAllAsync(search);
        return Ok(list);
    }

    /// <summary>Tìm khách hàng theo số điện thoại.</summary>
    [HttpGet("phone/{phone}")]
    public async Task<IActionResult> GetByPhone(string phone)
    {
        var customer = await _svc.GetByPhoneAsync(phone);
        return customer is null ? NotFound(new { message = "Không tìm thấy khách hàng." }) : Ok(customer);
    }
}
