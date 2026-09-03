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
                await _hr.ReviewRequestAsync(id, managerId, req.Status, req.Note);
                return Ok(new { message = req.Status == "DaDuyet" ? "Đã duyệt đơn từ thành công!" : "Đã từ chối đơn từ thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("review-checkin/{id:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> ReviewCheckIn(int id, [FromBody] ReviewCheckInPayload req)
        {
            try
            {
                var managerId = GetCurrentUserId();
                await _hr.ReviewCheckInAsync(id, managerId, req.Status, req.Note);
                return Ok(new { message = req.Status == "DaDuyet" || req.Status == "HopLe" ? "Đã duyệt công thành công!" : "Đã từ chối công thành công!" });
            }
            catch (Exception ex)
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

        [HttpPost("shifts")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> CreateShift([FromBody] SaveShiftDefinitionRequest req)
        {
            try
            {
                var data = await _hr.CreateShiftDefinitionAsync(req);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("shifts/{id:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> UpdateShift(int id, [FromBody] SaveShiftDefinitionRequest req)
        {
            try
            {
                await _hr.UpdateShiftDefinitionAsync(id, req);
                return Ok(new { message = "Đã cập nhật định nghĩa ca làm việc thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("shift-limits")]
        [Authorize]
        public async Task<IActionResult> GetShiftLimits()
        {
            var data = await _hr.GetShiftLimitsAsync();
            return Ok(data);
        }

        [HttpPost("shift-limits")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> SaveShiftLimits([FromBody] SaveShiftLimitsRequest req)
        {
            await _hr.SaveShiftLimitsAsync(req);
            return Ok(new { message = "Đã lưu cấu hình giới hạn ca thành công!" });
        }

        [HttpDelete("shifts/{id:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> DeleteShiftDefinition(int id)
        {
            await _hr.DeleteShiftDefinitionAsync(id);
            return Ok(new { message = "Đã xóa ca làm việc thành công!" });
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetActiveEmployees()
        {
            var data = await _hr.GetActiveEmployeesAsync();
            return Ok(data);
        }

        [HttpGet("schedules")]
        [Authorize]
        public async Task<IActionResult> GetSchedules()
        {
            var data = await _hr.GetSchedulesAsync();
            return Ok(data);
        }

        [HttpPost("schedules")]
        [Authorize]
        public async Task<IActionResult> CreateSchedule([FromBody] CreatePhanCaRequest req)
        {
            try
            {
                var data = await _hr.CreateScheduleAsync(req);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("schedules/{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            await _hr.DeleteScheduleAsync(id);
            return Ok(new { message = "Đã xóa ca phân công!" });
        }

        [HttpGet("payroll")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> GetPayrollSummary([FromQuery] string? ky)
        {
            var data = await _hr.GetPayrollSummaryAsync(ky);
            return Ok(data);
        }

        [HttpPut("employee-rate/{employeeId:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> UpdateEmployeeRate(int employeeId, [FromBody] UpdateEmployeeRateRequest req)
        {
            try
            {
                await _hr.UpdateEmployeeRateAsync(employeeId, req.LuongCoBan);
                return Ok(new { message = "Đã cập nhật mức lương cơ bản thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("pay-salary/{employeeId:int}")]
        [Authorize(Policy = Shared.Quyens.NhanSuQuanLy)]
        public async Task<IActionResult> PaySalary(int employeeId, [FromBody] PaySalaryRequest req)
        {
            try
            {
                var managerId = GetCurrentUserId();
                await _hr.PaySalaryAsync(employeeId, managerId, req);
                return Ok(new { message = "Đã xác nhận thanh toán lương cho nhân viên thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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
