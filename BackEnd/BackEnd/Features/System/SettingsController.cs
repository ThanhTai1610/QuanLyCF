using System.Security.Claims;
using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.System
{
    /// <summary>
    /// Quản lý cài đặt hệ thống:
    ///   GET  /api/settings/store-info   — thông tin quán công khai  (public)
    ///   GET  /api/settings              — đọc toàn bộ cài đặt      (CAIDAT_QUANLY)
    ///   PUT  /api/settings              — cập nhật cài đặt          (CAIDAT_QUANLY)
    ///   GET  /api/settings/maintenance  — trạng thái bảo trì        (public)
    /// </summary>
    [ApiController]
    [Route("api/settings")]
    [Authorize]
    public class SettingsController : ControllerBase
    {
        private readonly SettingService _svc;

        public SettingsController(SettingService svc) => _svc = svc;

        private int? CurrentUserId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub"), out var id)
                ? id : null;

        // GET /api/settings/store-info  (AllowAnonymous — thông tin hiển thị công khai)
        [HttpGet("store-info")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStoreInfo()
        {
            var dto = await _svc.GetStoreInfoAsync();
            return Ok(dto);
        }

        // GET /api/settings
        [HttpGet]
        [Authorize(Policy = Quyens.CaiDatQuanLy)]
        public async Task<IActionResult> GetSettings()
        {
            var dto = await _svc.GetAllAsync();
            return Ok(dto);
        }

        // PUT /api/settings
        [HttpPut]
        [Authorize(Policy = Quyens.CaiDatQuanLy)]
        public async Task<IActionResult> UpdateSettings(UpdateSettingsRequest req)
        {
            try
            {
                var ip = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
                var ua = Request.Headers["User-Agent"].ToString();
                await _svc.UpdateAsync(req, CurrentUserId, ip, ua);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET /api/settings/maintenance  (AllowAnonymous — dùng cho FE check bảo trì)
        [HttpGet("maintenance")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMaintenanceStatus()
        {
            var dto = await _svc.GetMaintenanceStatusAsync();
            return Ok(dto);
        }
    }
}
