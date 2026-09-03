using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest req)
    {
        Ban? ban = null;
        if (!string.IsNullOrEmpty(req.GhiChu))
        {
            var match = Regex.Match(req.GhiChu, @"Bàn\s+(\d+)", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var numStr = match.Groups[1].Value;
                ban = await _db.Bans.FirstOrDefaultAsync(b => b.TenBan == $"Bàn {numStr}" || b.TenBan == numStr);
            }
        }

        if (ban is null)
        {
            ban = await _db.Bans.FirstOrDefaultAsync(b => b.TenBan == $"Bàn {req.MaBan}" || b.TenBan == req.MaBan.ToString())
                ?? await _db.Bans.FindAsync(req.MaBan);
        }
        if (ban is null) return BadRequest(new { message = "Bàn không tồn tại." });

        // Chống tạo trùng lặp: nếu đã có Yêu cầu cùng bàn & loại chưa xử lý -> trả về yêu cầu cũ
        var existing = Requests.Values.FirstOrDefault(x => !x.DaXuLy && x.MaBan == ban.MaBan && x.LoaiYeuCau == req.LoaiYeuCau);
        if (existing is not null)
        {
            return Ok(existing);
        }

        var id = Guid.NewGuid().ToString("N");
        var item = new ServiceRequestDto(id, ban.MaBan, ban.TenBan, req.LoaiYeuCau, req.GhiChu, DateTime.UtcNow, false);
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
