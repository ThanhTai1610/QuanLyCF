using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd.Features.Sales.Tables;

public record CreateServiceRequest(int MaBan, string LoaiYeuCau, string? GhiChu);
public record ServiceRequestDto(string Id, int MaBan, string TenBan, string LoaiYeuCau, string? GhiChu, DateTime ThoiGianTao, bool DaXuLy);

[ApiController]
[Route("api/service-requests")]
public class ServiceRequestsController : ControllerBase
{
    private static readonly ConcurrentDictionary<string, ServiceRequestDto> Requests = new();
    private readonly QuanLyCFDbContext _db;

    public ServiceRequestsController(QuanLyCFDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateServiceRequest req)
    {
        var ban = await _db.Bans.FindAsync(req.MaBan);
        if (ban is null) return BadRequest(new { message = "Bàn không tồn tại." });

        var id = Guid.NewGuid().ToString("N");
        var item = new ServiceRequestDto(id, req.MaBan, ban.TenBan, req.LoaiYeuCau, req.GhiChu, DateTime.UtcNow, false);
        Requests[id] = item;
        return Ok(item);
    }

    [HttpGet("active")]
    [AllowAnonymous] // can be Authorize or AllowAnonymous, let's make it AllowAnonymous for easy local dashboard polling, or Authorize
    public IActionResult GetActive()
    {
        return Ok(Requests.Values.Where(x => !x.DaXuLy).OrderBy(x => x.ThoiGianTao).ToList());
    }

    [HttpPost("{id}/resolve")]
    [AllowAnonymous] // allow any client to resolve
    public IActionResult Resolve(string id)
    {
        if (Requests.TryGetValue(id, out var req))
        {
            Requests.TryRemove(id, out _);
            return NoContent();
        }
        return NotFound();
    }
}
