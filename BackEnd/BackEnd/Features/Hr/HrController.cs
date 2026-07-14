using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Hr
{
    [ApiController]
    [Route("api/hr")]
    [Authorize]
    public class HrController : ControllerBase
    {
        private readonly HrService _hr;

        public HrController(HrService hr)
        {
            _hr = hr;
        }

        [HttpGet("my-checkins")]
        public async Task<IActionResult> GetMyCheckIns([FromQuery] int? employeeId)
        {
            var userId = employeeId ?? GetCurrentUserId();
            var data = await _hr.GetMyCheckInsAsync(userId);
            return Ok(data);
        }

        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn([FromBody] CheckInRequest req)
        {
            try
            {
                var userId = req.MaNhanVien ?? GetCurrentUserId();
                var cc = await _hr.CheckInAsync(userId, req);
                return Ok(new { message = req.Type.ToLower() == "in" ? "Vào ca thành công!" : "Kết ca thành công!", id = cc.MaChamCong });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = GetCurrentUserId();
            var data = await _hr.GetMyRequestsAsync(userId);
            return Ok(data);
        }

        [HttpPost("create-request")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateRequest req)
        {
            var userId = req.MaNhanVien ?? GetCurrentUserId();
            var don = await _hr.CreateRequestAsync(userId, req);
            return Ok(new { message = "Đã gửi đơn yêu cầu thành công!", id = don.MaDon });
        }

        [HttpGet("active-checkins")]
        [Authorize(Policy = Shared.Quyens.NhanSuXem)]
        public async Task<IActionResult> GetActiveCheckIns()
        {
            var data = await _hr.GetActiveCheckInsAsync();
            return Ok(data);
        }

        [HttpPost("force-checkout/{id:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> ForceCheckOut(int id, [FromBody] ForceCheckOutRequest req)
        {
            try
            {
                var managerName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin";
                await _hr.ForceCheckOutAsync(id, managerName, req.Reason);
                return Ok(new { message = "Đã kết ca thành công cho nhân viên!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all-requests")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> GetAllRequests()
        {
            var data = await _hr.GetAllRequestsAsync();
            return Ok(data);
        }

        [HttpPost("review-request/{id:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> ReviewRequest(int id, [FromBody] ReviewRequestPayload req)
        {
            try
            {
                var managerId = GetCurrentUserId();
                await _hr.ReviewRequestAsync(id, managerId, req.Status);
                return Ok(new { message = "Đã cập nhật trạng thái đơn từ thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("shifts")]
        public async Task<IActionResult> GetActiveShifts()
        {
            var data = await _hr.GetActiveShiftsAsync();
            return Ok(data);
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetActiveEmployees()
        {
            var data = await _hr.GetActiveEmployeesAsync();
            return Ok(data);
        }

        private int GetCurrentUserId()
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(idStr))
            {
                throw new UnauthorizedAccessException("Không xác định được danh tính nhân viên.");
            }
            return int.Parse(idStr);
        }
    }
}
