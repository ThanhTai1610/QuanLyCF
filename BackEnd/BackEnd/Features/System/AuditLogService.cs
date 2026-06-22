using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.System
{
    /// <summary>Service đọc nhật ký hệ thống (chỉ đọc, không ghi trực tiếp).</summary>
    public class AuditLogService
    {
        private readonly QuanLyCFDbContext _db;
        public AuditLogService(QuanLyCFDbContext db) => _db = db;

        /// <summary>
        /// Lấy danh sách nhật ký với bộ lọc tuỳ chọn.
        /// </summary>
        public async Task<(List<AuditLogItem> Items, int TotalCount)> GetPagedAsync(AuditLogQuery q)
        {
            var query = _db.NhatKyHeThongs
                .Include(x => x.NhanVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q.Module))
                query = query.Where(x => x.Module == q.Module);

            if (!string.IsNullOrWhiteSpace(q.HanhDong))
                query = query.Where(x => x.HanhDong == q.HanhDong);

            if (q.MaNhanVien.HasValue)
                query = query.Where(x => x.MaNhanVien == q.MaNhanVien.Value);

            int total = await query.CountAsync();

            int pageSize = Math.Clamp(q.PageSize, 1, 100);
            int page     = Math.Max(q.Page, 1);

            var items = await query
                .OrderByDescending(x => x.ThoiGianTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new AuditLogItem(
                    x.MaNhatKy,
                    x.MaNhanVien,
                    x.NhanVien != null ? x.NhanVien.HoTen : null,
                    x.HanhDong,
                    x.Module,
                    x.DuLieuCu,
                    x.DuLieuMoi,
                    x.DiaChiIP,
                    x.ThietBi,
                    x.ThoiGianTao
                ))
                .ToListAsync();

            return (items, total);
        }

        /// <summary>Danh sách các module đã có nhật ký (dùng cho filter FE).</summary>
        public async Task<List<string>> GetModulesAsync() =>
            await _db.NhatKyHeThongs
                .Select(x => x.Module)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
    }
}
