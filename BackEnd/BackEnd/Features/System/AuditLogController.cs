using BackEnd.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.System
{
    /// <summary>
    /// Nhật ký hệ thống (Audit Log):
    ///   GET /api/audit-logs                — danh sách nhật ký có lọc + phân trang  (CAIDAT_QUANLY)
    ///   GET /api/audit-logs/modules        — danh sách module đã có nhật ký          (CAIDAT_QUANLY)
    /// </summary>
    [ApiController]
    [Route("api/audit-logs")]
    [Authorize(Policy = Quyens.CaiDatQuanLy)]
    public class AuditLogController : ControllerBase
    {
        private readonly AuditLogService _svc;

        public AuditLogController(AuditLogService svc) => _svc = svc;

        // GET /api/audit-logs?module=&hanhDong=&maNhanVien=&page=1&pageSize=20
        [HttpGet]
        public async Task<IActionResult> GetLogs(
            [FromQuery] string? module,
            [FromQuery] string? hanhDong,
            [FromQuery] int?    maNhanVien,
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 20)
        {
            var query = new AuditLogQuery(module, hanhDong, maNhanVien, page, pageSize);
            var (items, total) = await _svc.GetPagedAsync(query);

            return Ok(new
            {
                data       = items,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        // GET /api/audit-logs/modules
        [HttpGet("modules")]
        public async Task<IActionResult> GetModules()
        {
            var modules = await _svc.GetModulesAsync();
            return Ok(modules);
        }
    }
}
