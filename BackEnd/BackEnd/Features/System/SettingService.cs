using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BackEnd.Features.System
{
    /// <summary>
    /// Service xử lý logic nghiệp vụ cho tính năng Cài đặt hệ thống.
    /// Controller chỉ nhận request → gọi service → trả response.
    /// </summary>
    public class SettingService
    {
        private readonly QuanLyCFDbContext _db;
        private readonly IMemoryCache      _cache;

        // Các khoá cài đặt
        private static readonly string[] AllKeys =
        {
            "TEN_QUAN", "DIA_CHI", "SO_DIEN_THOAI", "MO_TA_QUAN",
            "THUE_VAT_MAC_DINH", "PHI_DICH_VU", "TY_LE_TICH_DIEM",
            "CHE_DO_BAO_TRI", "THONG_DIEP_BAO_TRI"
        };

        private static readonly Dictionary<string, string> NhomMap = new()
        {
            ["TEN_QUAN"]           = "CHUNG",
            ["DIA_CHI"]            = "CHUNG",
            ["SO_DIEN_THOAI"]      = "CHUNG",
            ["MO_TA_QUAN"]         = "CHUNG",
            ["CHE_DO_BAO_TRI"]     = "CHUNG",
            ["THONG_DIEP_BAO_TRI"] = "CHUNG",
            ["THUE_VAT_MAC_DINH"]  = "THANH_TOAN",
            ["PHI_DICH_VU"]        = "THANH_TOAN",
            ["TY_LE_TICH_DIEM"]    = "TICH_DIEM",
        };

        public SettingService(QuanLyCFDbContext db, IMemoryCache cache)
        {
            _db    = db;
            _cache = cache;
        }

        // ── Đọc toàn bộ cài đặt ───────────────────────────────────────────────

        public async Task<SystemSettingsDto> GetAllAsync()
        {
            var list = await _db.CaiDatHeThongs
                .Where(x => AllKeys.Contains(x.KhoaCaiDat))
                .ToListAsync();

            string Val(string key, string def = "") =>
                list.FirstOrDefault(x => x.KhoaCaiDat == key)?.GiaTriCaiDat ?? def;

            return new SystemSettingsDto(
                TenQuan:          Val("TEN_QUAN", "BrewManager Coffee"),
                DiaChi:           Val("DIA_CHI"),
                SoDienThoai:      Val("SO_DIEN_THOAI"),
                MoTaQuan:         Val("MO_TA_QUAN"),
                ThueVatMacDinh:   Val("THUE_VAT_MAC_DINH", "8"),
                PhiDichVu:        Val("PHI_DICH_VU", "0"),
                TyLeTichDiem:     Val("TY_LE_TICH_DIEM", "1"),
                CheDoBaoTri:      Val("CHE_DO_BAO_TRI", "false")
                                    .Equals("true", StringComparison.OrdinalIgnoreCase),
                ThongDiepBaoTri:  Val("THONG_DIEP_BAO_TRI",
                                    "Hệ thống đang bảo trì. Vui lòng quay lại sau.")
            );
        }

        // ── Cập nhật cài đặt ──────────────────────────────────────────────────

        public async Task UpdateAsync(UpdateSettingsRequest req)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(req.TenQuan))
                throw new ArgumentException("Tên quán không được để trống.");
            if (!decimal.TryParse(req.ThueVatMacDinh, out var vat) || vat < 0 || vat > 100)
                throw new ArgumentException("Thuế VAT phải là số từ 0 đến 100.");
            if (!decimal.TryParse(req.PhiDichVu, out _))
                throw new ArgumentException("Phí dịch vụ phải là số hợp lệ.");
            if (!decimal.TryParse(req.TyLeTichDiem, out _))
                throw new ArgumentException("Tỷ lệ tích điểm phải là số hợp lệ.");

            var payload = new Dictionary<string, string?>
            {
                ["TEN_QUAN"]           = req.TenQuan.Trim(),
                ["DIA_CHI"]            = req.DiaChi?.Trim(),
                ["SO_DIEN_THOAI"]      = req.SoDienThoai?.Trim(),
                ["MO_TA_QUAN"]         = req.MoTaQuan?.Trim(),
                ["THUE_VAT_MAC_DINH"]  = req.ThueVatMacDinh.Trim(),
                ["PHI_DICH_VU"]        = req.PhiDichVu.Trim(),
                ["TY_LE_TICH_DIEM"]    = req.TyLeTichDiem.Trim(),
                ["CHE_DO_BAO_TRI"]     = req.CheDoBaoTri ? "true" : "false",
                ["THONG_DIEP_BAO_TRI"] = req.ThongDiepBaoTri?.Trim(),
            };

            var existing = await _db.CaiDatHeThongs
                .Where(x => payload.Keys.Contains(x.KhoaCaiDat))
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var (key, value) in payload)
            {
                var row = existing.FirstOrDefault(x => x.KhoaCaiDat == key);
                if (row is not null)
                {
                    row.GiaTriCaiDat  = value;
                    row.ThoiGianCapNhat = now;
                }
                else
                {
                    _db.CaiDatHeThongs.Add(new CaiDatHeThong
                    {
                        NhomCaiDat      = NhomMap.GetValueOrDefault(key, "CHUNG"),
                        KhoaCaiDat      = key,
                        GiaTriCaiDat    = value,
                        ThoiGianCapNhat = now,
                    });
                }
            }

            await _db.SaveChangesAsync();

            // Xoá cache bảo trì ngay lập tức khi cập nhật cài đặt
            _cache.Remove("__maint_on");
            _cache.Remove("__maint_msg");
        }

        // ── Thông tin công khai của quán (public) ────────────────────────────

        public async Task<StoreInfoDto> GetStoreInfoAsync()
        {
            var publicKeys = new[] { "TEN_QUAN", "DIA_CHI", "SO_DIEN_THOAI", "MO_TA_QUAN" };
            var rows = await _db.CaiDatHeThongs
                .AsNoTracking()
                .Where(x => publicKeys.Contains(x.KhoaCaiDat))
                .ToListAsync();

            string Val(string key, string def = "") =>
                rows.FirstOrDefault(x => x.KhoaCaiDat == key)?.GiaTriCaiDat ?? def;

            return new StoreInfoDto(
                TenQuan:     Val("TEN_QUAN", "BrewManager Coffee"),
                DiaChi:      Val("DIA_CHI"),
                SoDienThoai: Val("SO_DIEN_THOAI"),
                MoTaQuan:    Val("MO_TA_QUAN")
            );
        }

        // ── Trạng thái bảo trì (public) ───────────────────────────────────────

        public async Task<MaintenanceStatusDto> GetMaintenanceStatusAsync()
        {
            var rows = await _db.CaiDatHeThongs
                .Where(x => x.KhoaCaiDat == "CHE_DO_BAO_TRI"
                         || x.KhoaCaiDat == "THONG_DIEP_BAO_TRI")
                .ToListAsync();

            bool isMaint = (rows.FirstOrDefault(x => x.KhoaCaiDat == "CHE_DO_BAO_TRI")
                              ?.GiaTriCaiDat ?? "false")
                           .Equals("true", StringComparison.OrdinalIgnoreCase);

            string msg = rows.FirstOrDefault(x => x.KhoaCaiDat == "THONG_DIEP_BAO_TRI")
                           ?.GiaTriCaiDat
                         ?? "Hệ thống đang bảo trì. Vui lòng quay lại sau.";

            return new MaintenanceStatusDto(isMaint, msg);
        }
    }
}
