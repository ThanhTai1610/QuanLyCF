using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Hr
{
    public class HrService
    {
        private readonly QuanLyCFDbContext _db;

        public HrService(QuanLyCFDbContext db)
        {
            _db = db;
        }

        public async Task<List<ChamCongResponse>> GetMyCheckInsAsync(int userId)
        {
            var logs = await _db.ChamCongs
                .Include(x => x.Ca)
                .Where(x => x.MaNhanVien == userId)
                .OrderByDescending(x => x.ThoiGianVao)
                .Take(50)
                .ToListAsync();

            var result = new List<ChamCongResponse>();
            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // GMT+7

            foreach (var log in logs)
            {
                var timeInLocal = TimeZoneInfo.ConvertTimeFromUtc(log.ThoiGianVao, tz);
                var timeOutLocal = log.ThoiGianRa.HasValue 
                    ? TimeZoneInfo.ConvertTimeFromUtc(log.ThoiGianRa.Value, tz) 
                    : (DateTime?)null;

                var totalStr = "";
                if (log.ThoiGianRa.HasValue)
                {
                    var diff = log.ThoiGianRa.Value - log.ThoiGianVao;
                    totalStr = $"{(int)diff.TotalHours}h {diff.Minutes}m";
                }

                result.Add(new ChamCongResponse
                {
                    MaChamCong = log.MaChamCong,
                    MaCa = log.MaCa,
                    TenCa = log.Ca?.TenCa ?? "Ca Tự Do",
                    Date = FormatDateFriendly(timeInLocal),
                    TimeIn = timeInLocal.ToString("HH:mm"),
                    TimeOut = timeOutLocal?.ToString("HH:mm") ?? "",
                    ImgIn = log.AnhVao,
                    ImgOut = log.AnhRa,
                    TimeInExact = timeInLocal.ToString("HH:mm:ss"),
                    TimeOutExact = timeOutLocal?.ToString("HH:mm:ss") ?? "",
                    Total = totalStr,
                    GhiChu = log.GhiChu
                });
            }

            return result;
        }

        public async Task<ChamCong> CheckInAsync(int userId, CheckInRequest req)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // GMT+7
            var utcNow = DateTime.UtcNow;
            var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, tz);

            if (req.Type.ToLower() == "in")
            {
                // Kiểm tra xem đã có lượt check-in nào chưa kết ca trong vòng 18h qua không
                var cutoff = utcNow.AddHours(-18);
                var active = await _db.ChamCongs
                    .FirstOrDefaultAsync(x => x.MaNhanVien == userId && x.ThoiGianVao >= cutoff && x.ThoiGianRa == null);

                if (active != null)
                {
                    throw new InvalidOperationException("Bạn đã vào ca trước đó rồi. Vui lòng kết ca trước.");
                }

                int? maCa = req.MaCa;
                if (!maCa.HasValue)
                {
                    // Tự động tìm ca phù hợp dựa trên thời gian hiện tại (hỗ trợ ca qua đêm)
                    var timeOnly = TimeOnly.FromDateTime(localNow);
                    var matchingCa = await _db.CaLamViecs
                        .FirstOrDefaultAsync(c => c.TrangThaiHoatDong && 
                            ((c.GioBatDau <= c.GioKetThuc && c.GioBatDau <= timeOnly && timeOnly <= c.GioKetThuc) ||
                             (c.GioBatDau > c.GioKetThuc && (timeOnly >= c.GioBatDau || timeOnly <= c.GioKetThuc))));
                    maCa = matchingCa?.MaCa;
                }

                var cc = new ChamCong
                {
                    MaNhanVien = userId,
                    MaCa = maCa,
                    ThoiGianVao = utcNow,
                    AnhVao = req.PhotoUrl,
                    GhiChu = req.GhiChu,
                    TrangThai = "HopLe"
                };

                // Tính toán đi trễ (hỗ trợ ca qua đêm)
                if (cc.MaCa.HasValue)
                {
                    var ca = await _db.CaLamViecs.FindAsync(cc.MaCa.Value);
                    if (ca != null)
                    {
                        var gioVaoLocal = TimeOnly.FromDateTime(localNow);
                        if (ca.GioBatDau <= ca.GioKetThuc)
                        {
                            if (gioVaoLocal > ca.GioBatDau)
                            {
                                var tre = (int)(gioVaoLocal - ca.GioBatDau).TotalMinutes;
                                cc.SoPhutDiTre = tre > 0 ? tre : 0;
                            }
                        }
                        else
                        {
                            var diffMinutes = (int)(gioVaoLocal - ca.GioBatDau).TotalMinutes;
                            if (diffMinutes < 0) diffMinutes += 1440;
                            if (diffMinutes > 0 && diffMinutes < 720) // Đi trễ trong vòng 12 tiếng đầu ca
                            {
                                cc.SoPhutDiTre = diffMinutes;
                            }
                        }
                    }
                }

                _db.ChamCongs.Add(cc);
                await _db.SaveChangesAsync();
                return cc;
            }
            else if (req.Type.ToLower() == "out")
            {
                var cc = await _db.ChamCongs
                    .Where(x => x.MaNhanVien == userId && x.ThoiGianRa == null)
                    .OrderByDescending(x => x.ThoiGianVao)
                    .FirstOrDefaultAsync();

                if (cc == null)
                {
                    throw new InvalidOperationException("Không tìm thấy lượt vào ca chưa kết ca của bạn.");
                }

                cc.ThoiGianRa = utcNow;
                cc.AnhRa = req.PhotoUrl;
                if (!string.IsNullOrEmpty(req.GhiChu))
                {
                    cc.GhiChu = string.IsNullOrEmpty(cc.GhiChu) ? req.GhiChu : cc.GhiChu + " | " + req.GhiChu;
                }

                // Tính toán về sớm (hỗ trợ ca qua đêm)
                if (cc.MaCa.HasValue)
                {
                    var ca = await _db.CaLamViecs.FindAsync(cc.MaCa.Value);
                    if (ca != null)
                    {
                        var gioRaLocal = TimeOnly.FromDateTime(localNow);
                        if (ca.GioBatDau <= ca.GioKetThuc)
                        {
                            if (gioRaLocal < ca.GioKetThuc)
                            {
                                var som = (int)(ca.GioKetThuc - gioRaLocal).TotalMinutes;
                                cc.SoPhutVeSom = som > 0 ? som : 0;
                            }
                        }
                        else
                        {
                            var diffMinutes = (int)(ca.GioKetThuc - gioRaLocal).TotalMinutes;
                            if (diffMinutes < 0) diffMinutes += 1440;
                            if (diffMinutes > 0 && diffMinutes < 720) // Về sớm trong vòng 12 tiếng cuối ca
                            {
                                cc.SoPhutVeSom = diffMinutes;
                            }
                        }
                    }
                }

                await _db.SaveChangesAsync();
                return cc;
            }
            else
            {
                throw new ArgumentException("Loại chấm công không hợp lệ.");
            }
        }

        public async Task<List<DonTuResponse>> GetMyRequestsAsync(int userId)
        {
            var list = await _db.DonTuNhanViens
                .Where(x => x.MaNhanVien == userId)
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // GMT+7

            return list.Select(x => new DonTuResponse
            {
                MaDon = x.MaDon,
                LoaiDon = MapLoaiDon(x.LoaiDon),
                ThoiGianLienQuan = x.ThoiGianLienQuan ?? "",
                LyDo = x.LyDo ?? "",
                TrangThai = x.TrangThai,
                ThoiGianTao = TimeZoneInfo.ConvertTimeFromUtc(x.ThoiGianTao, tz).ToString("dd/MM/yyyy HH:mm")
            }).ToList();
        }

        public async Task<DonTuNhanVien> CreateRequestAsync(int userId, CreateRequest req)
        {
            var don = new DonTuNhanVien
            {
                MaNhanVien = userId,
                LoaiDon = req.LoaiDon,
                ThoiGianLienQuan = req.ThoiGianLienQuan,
                LyDo = req.LyDo,
                TrangThai = "ChoDuyet",
                ThoiGianTao = DateTime.UtcNow
            };

            _db.DonTuNhanViens.Add(don);
            await _db.SaveChangesAsync();
            return don;
        }

        public async Task<List<ChamCongResponse>> GetActiveCheckInsAsync()
        {
            var logs = await _db.ChamCongs
                .Include(x => x.NhanVien)
                .Include(x => x.Ca)
                .Where(x => x.ThoiGianRa == null)
                .OrderByDescending(x => x.ThoiGianVao)
                .ToListAsync();

            var result = new List<ChamCongResponse>();
            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // GMT+7

            foreach (var log in logs)
            {
                var timeInLocal = TimeZoneInfo.ConvertTimeFromUtc(log.ThoiGianVao, tz);
                result.Add(new ChamCongResponse
                {
                    MaChamCong = log.MaChamCong,
                    MaCa = log.MaCa,
                    TenCa = $"{log.NhanVien.HoTen} - {log.Ca?.TenCa ?? "Ca Tự Do"}",
                    Date = FormatDateFriendly(timeInLocal),
                    TimeIn = timeInLocal.ToString("HH:mm"),
                    TimeOut = "",
                    ImgIn = log.AnhVao,
                    ImgOut = "",
                    TimeInExact = timeInLocal.ToString("HH:mm:ss"),
                    TimeOutExact = "",
                    Total = "",
                    GhiChu = log.GhiChu
                });
            }

            return result;
        }

        public async Task ForceCheckOutAsync(int checkInId, string managerName, string? reason)
        {
            var cc = await _db.ChamCongs.FindAsync(checkInId);
            if (cc == null)
            {
                throw new InvalidOperationException("Không tìm thấy dòng chấm công.");
            }

            if (cc.ThoiGianRa != null)
            {
                throw new InvalidOperationException("Ca làm việc này đã được kết ca trước đó.");
            }

            cc.ThoiGianRa = DateTime.UtcNow;
            var suffix = string.IsNullOrEmpty(reason) ? "" : $" (Lý do: {reason})";
            cc.GhiChu = string.IsNullOrEmpty(cc.GhiChu) 
                ? $"Quản lý {managerName} kết ca hộ{suffix}" 
                : cc.GhiChu + $" | Quản lý {managerName} kết ca hộ{suffix}";

            // Tính toán về sớm nếu có ca
            if (cc.MaCa.HasValue)
            {
                var ca = await _db.CaLamViecs.FindAsync(cc.MaCa.Value);
                if (ca != null)
                {
                    var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                    var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tz);
                    var gioRaLocal = TimeOnly.FromDateTime(localNow);
                    if (ca.GioBatDau <= ca.GioKetThuc)
                    {
                        if (gioRaLocal < ca.GioKetThuc)
                        {
                            var som = (int)(ca.GioKetThuc - gioRaLocal).TotalMinutes;
                            cc.SoPhutVeSom = som > 0 ? som : 0;
                        }
                    }
                    else
                    {
                        var diffMinutes = (int)(ca.GioKetThuc - gioRaLocal).TotalMinutes;
                        if (diffMinutes < 0) diffMinutes += 1440;
                        if (diffMinutes > 0 && diffMinutes < 720)
                        {
                            cc.SoPhutVeSom = diffMinutes;
                        }
                    }
                }
            }

            await _db.SaveChangesAsync();
        }

        private string FormatDateFriendly(DateTime dt)
        {
            var today = DateTime.Today;
            if (dt.Date == today) return $"Hôm nay, {dt:dd/MM}";
            if (dt.Date == today.AddDays(-1)) return $"Hôm qua, {dt:dd/MM}";

            var dayOfWeekStr = dt.DayOfWeek switch
            {
                DayOfWeek.Monday => "T2",
                DayOfWeek.Tuesday => "T3",
                DayOfWeek.Wednesday => "T4",
                DayOfWeek.Thursday => "T5",
                DayOfWeek.Friday => "T6",
                DayOfWeek.Saturday => "T7",
                DayOfWeek.Sunday => "CN",
                _ => ""
            };
            return $"{dayOfWeekStr}, {dt:dd/MM}";
        }

        private string MapLoaiDon(string code)
        {
            return code switch
            {
                "PhepNam" => "Phép năm",
                "TangCa" => "Tăng ca",
                "NghiKhongLuong" => "Nghỉ không lương",
                "NghiBu" => "Nghỉ bù",
                _ => code
            };
        }

        public async Task<List<DonTuResponse>> GetAllRequestsAsync()
        {
            var list = await _db.DonTuNhanViens
                .Include(x => x.NhanVien)
                .OrderByDescending(x => x.ThoiGianTao)
                .ToListAsync();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // GMT+7

            return list.Select(x => new DonTuResponse
            {
                MaDon = x.MaDon,
                LoaiDon = MapLoaiDon(x.LoaiDon),
                ThoiGianLienQuan = x.ThoiGianLienQuan ?? "",
                LyDo = x.LyDo ?? "",
                TrangThai = x.TrangThai,
                ThoiGianTao = TimeZoneInfo.ConvertTimeFromUtc(x.ThoiGianTao, tz).ToString("dd/MM/yyyy HH:mm"),
                TenNhanVien = x.NhanVien.HoTen
            }).ToList();
        }

        public async Task ReviewRequestAsync(int requestId, int managerId, string status)
        {
            var don = await _db.DonTuNhanViens.FindAsync(requestId);
            if (don == null)
            {
                throw new InvalidOperationException("Không tìm thấy đơn từ.");
            }

            if (don.TrangThai != "ChoDuyet")
            {
                throw new InvalidOperationException("Đơn từ này đã được duyệt hoặc từ chối trước đó.");
            }

            if (status != "DaDuyet" && status != "TuChoi")
            {
                throw new ArgumentException("Trạng thái duyệt không hợp lệ.");
            }

            don.TrangThai = status;
            don.MaNguoiDuyet = managerId;

            await _db.SaveChangesAsync();
        }

        public async Task<List<CaLamViecResponse>> GetActiveShiftsAsync()
        {
            var list = await _db.CaLamViecs
                .Where(x => x.TrangThaiHoatDong)
                .ToListAsync();

            return list.Select(x => new CaLamViecResponse
            {
                MaCa = x.MaCa,
                TenCa = x.TenCa,
                GioBatDau = x.GioBatDau.ToString("HH:mm"),
                GioKetThuc = x.GioKetThuc.ToString("HH:mm")
            }).ToList();
        }

        public async Task<List<EmployeeResponse>> GetActiveEmployeesAsync()
        {
            var list = await _db.NhanViens
                .Where(x => x.TrangThaiHoatDong)
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            return list.Select(x => new EmployeeResponse
            {
                MaNhanVien = x.MaNhanVien,
                HoTen = x.HoTen
            }).ToList();
        }
    }
}
