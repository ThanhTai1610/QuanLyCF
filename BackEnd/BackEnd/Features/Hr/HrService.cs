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
                    GhiChu = log.GhiChu,
                    TrangThai = log.TrangThai ?? "ChoDuyet"
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

                // Ghi Nhật Ký Hệ Thống (Audit Log)
                var nvInfo = await _db.NhanViens.FindAsync(userId);
                var caInfo = cc.MaCa.HasValue ? await _db.CaLamViecs.FindAsync(cc.MaCa.Value) : null;
                var nvName = nvInfo?.HoTen ?? $"Mã NV: {userId}";
                var caName = caInfo?.TenCa ?? "Ca tự do";
                var isTre = cc.SoPhutDiTre >= 5;
                var detailText = isTre 
                    ? $"Mới: Nhân viên {nvName} vừa chấm công VÀO ca [{caName}] TRỄ {cc.SoPhutDiTre} phút." + (string.IsNullOrWhiteSpace(cc.GhiChu) ? "" : $" Lý do đi trễ: {cc.GhiChu}")
                    : $"Mới: Nhân viên {nvName} vừa chấm công VÀO ca [{caName}] đúng giờ.";

                _db.NhatKyHeThongs.Add(new Domain.Entities.NhatKyHeThong
                {
                    MaNhanVien = userId,
                    HanhDong = isTre ? "ĐI TRỄ" : "CHẤM CÔNG VÀO",
                    Module = "CHẤM CÔNG",
                    DuLieuMoi = detailText,
                    ThietBi = "Màn hình Chấm Công",
                    ThoiGianTao = DateTime.UtcNow
                });

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

                // Ghi Nhật Ký Hệ Thống (Audit Log)
                var nvInfo = await _db.NhanViens.FindAsync(userId);
                var caInfo = cc.MaCa.HasValue ? await _db.CaLamViecs.FindAsync(cc.MaCa.Value) : null;
                var nvName = nvInfo?.HoTen ?? $"Mã NV: {userId}";
                var caName = caInfo?.TenCa ?? "Ca tự do";

                _db.NhatKyHeThongs.Add(new Domain.Entities.NhatKyHeThong
                {
                    MaNhanVien = userId,
                    HanhDong = "CHẤM CÔNG RA",
                    Module = "CHẤM CÔNG",
                    DuLieuMoi = $"Mới: Nhân viên {nvName} vừa chấm công RA ca [{caName}].",
                    ThietBi = "Màn hình Chấm Công",
                    ThoiGianTao = DateTime.UtcNow
                });

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
                ThoiGianTao = TimeZoneInfo.ConvertTimeFromUtc(x.ThoiGianTao, tz).ToString("dd/MM/yyyy HH:mm"),
                GhiChuDuyet = x.GhiChuDuyet
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
                    GhiChu = log.GhiChu,
                    TrangThai = log.TrangThai ?? "ChoDuyet"
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
                TenNhanVien = x.NhanVien.HoTen,
                GhiChuDuyet = x.GhiChuDuyet
            }).ToList();
        }

        public async Task ReviewRequestAsync(int requestId, int managerId, string status, string? note)
        {
            var don = await _db.DonTuNhanViens
                .Include(x => x.NhanVien)
                .FirstOrDefaultAsync(x => x.MaDon == requestId);

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

            if (status == "TuChoi" && string.IsNullOrWhiteSpace(note))
            {
                throw new InvalidOperationException("Khi từ chối đơn, Quản lý/Admin bắt buộc phải nhập lý do từ chối!");
            }

            don.TrangThai = status;
            don.MaNguoiDuyet = managerId;
            don.GhiChuDuyet = note;

            var manager = await _db.NhanViens.FindAsync(managerId);
            var managerName = manager?.HoTen ?? "Quản lý";
            var empName = don.NhanVien?.HoTen ?? $"NV #{don.MaNhanVien}";
            var actionText = status == "DaDuyet" ? "DUYỆT ĐƠN TỪ" : "TỪ CHỐI ĐƠN TỪ";
            var detailText = $"Quản lý {managerName} đã {(status == "DaDuyet" ? "DUYỆT" : "TỪ CHỐI")} đơn ({don.LoaiDon}) cho nhân viên {empName}." + (string.IsNullOrWhiteSpace(note) ? "" : $" LÝ DO: {note}");

            _db.NhatKyHeThongs.Add(new Domain.Entities.NhatKyHeThong
            {
                MaNhanVien = managerId,
                HanhDong = actionText,
                Module = "QUẢN LÝ ĐƠN TỪ",
                DuLieuMoi = detailText,
                ThietBi = "Màn hình Quản lý HR",
                ThoiGianTao = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public async Task ReviewCheckInAsync(int checkInId, int managerId, string status, string? note)
        {
            var cc = await _db.ChamCongs
                .Include(x => x.NhanVien)
                .Include(x => x.Ca)
                .FirstOrDefaultAsync(x => x.MaChamCong == checkInId);

            if (cc == null)
            {
                throw new InvalidOperationException("Không tìm thấy dữ liệu chấm công.");
            }

            if (status != "DaDuyet" && status != "TuChoi" && status != "HopLe" && status != "KhongHopLe")
            {
                throw new ArgumentException("Trạng thái duyệt không hợp lệ.");
            }

            cc.TrangThai = status;
            if (!string.IsNullOrWhiteSpace(note))
            {
                cc.GhiChu = string.IsNullOrWhiteSpace(cc.GhiChu) ? note : cc.GhiChu + $" | Ghi chú duyệt: {note}";
            }

            var manager = await _db.NhanViens.FindAsync(managerId);
            var managerName = manager?.HoTen ?? "Quản lý";
            var empName = cc.NhanVien?.HoTen ?? $"NV #{cc.MaNhanVien}";
            var caName = cc.Ca?.TenCa ?? "Ca tự do";

            var isApprove = status == "DaDuyet" || status == "HopLe";
            var actionText = isApprove ? "DUYỆT CÔNG" : "TỪ CHỐI CÔNG";
            var detailText = $"Quản lý {managerName} đã {(isApprove ? "DUYỆT" : "TỪ CHỐI")} công cho nhân viên {empName} ({caName})." + (string.IsNullOrWhiteSpace(note) ? "" : $" Ghi chú: {note}");

            _db.NhatKyHeThongs.Add(new Domain.Entities.NhatKyHeThong
            {
                MaNhanVien = managerId,
                HanhDong = actionText,
                Module = "CHẤM CÔNG",
                DuLieuMoi = detailText,
                ThietBi = "Màn hình Quản lý HR",
                ThoiGianTao = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }

        public async Task<List<CaLamViecResponse>> GetActiveShiftsAsync()
        {
            var list = await _db.CaLamViecs
                .Where(x => x.TrangThaiHoatDong)
                .OrderBy(x => x.GioBatDau)
                .ToListAsync();

            return list.Select(x => new CaLamViecResponse
            {
                MaCa = x.MaCa,
                TenCa = x.TenCa,
                GioBatDau = x.GioBatDau.ToString("HH:mm"),
                GioKetThuc = x.GioKetThuc.ToString("HH:mm")
            }).ToList();
        }

        public async Task<CaLamViecResponse> CreateShiftDefinitionAsync(SaveShiftDefinitionRequest req)
        {
            if (!TimeOnly.TryParse(req.GioBatDau, out var bd) || !TimeOnly.TryParse(req.GioKetThuc, out var kt))
            {
                throw new ArgumentException("Giờ bắt đầu hoặc giờ kết thúc không đúng định dạng (HH:mm).");
            }

            var ca = new CaLamViec
            {
                TenCa = req.TenCa.Trim(),
                GioBatDau = bd,
                GioKetThuc = kt,
                TrangThaiHoatDong = true
            };

            _db.CaLamViecs.Add(ca);
            await _db.SaveChangesAsync();

            return new CaLamViecResponse
            {
                MaCa = ca.MaCa,
                TenCa = ca.TenCa,
                GioBatDau = ca.GioBatDau.ToString("HH:mm"),
                GioKetThuc = ca.GioKetThuc.ToString("HH:mm")
            };
        }

        public async Task UpdateShiftDefinitionAsync(int id, SaveShiftDefinitionRequest req)
        {
            var ca = await _db.CaLamViecs.FindAsync(id);
            if (ca == null) throw new InvalidOperationException("Không tìm thấy ca làm việc.");

            if (!TimeOnly.TryParse(req.GioBatDau, out var bd) || !TimeOnly.TryParse(req.GioKetThuc, out var kt))
            {
                throw new ArgumentException("Giờ bắt đầu hoặc giờ kết thúc không đúng định dạng (HH:mm).");
            }

            ca.TenCa = req.TenCa.Trim();
            ca.GioBatDau = bd;
            ca.GioKetThuc = kt;

            await _db.SaveChangesAsync();
        }

        public async Task<ShiftLimitsResponse> GetShiftLimitsAsync()
        {
            var rowGen = await _db.CaiDatHeThongs.FirstOrDefaultAsync(x => x.KhoaCaiDat == "SHIFT_LIMITS_GEN");
            var rowDaily = await _db.CaiDatHeThongs.FirstOrDefaultAsync(x => x.KhoaCaiDat == "SHIFT_LIMITS_DAILY");

            return new ShiftLimitsResponse
            {
                GeneralLimitsJson = rowGen?.GiaTriCaiDat ?? "{}",
                DailyLimitsJson = rowDaily?.GiaTriCaiDat ?? "{}"
            };
        }

        public async Task SaveShiftLimitsAsync(SaveShiftLimitsRequest req)
        {
            var rowGen = await _db.CaiDatHeThongs.FirstOrDefaultAsync(x => x.KhoaCaiDat == "SHIFT_LIMITS_GEN");
            if (rowGen == null)
            {
                rowGen = new Domain.Entities.CaiDatHeThong
                {
                    NhomCaiDat = "NHAN_SU",
                    KhoaCaiDat = "SHIFT_LIMITS_GEN",
                    GiaTriCaiDat = req.GeneralLimitsJson ?? "{}",
                    MoTa = "Cấu hình giới hạn ca chung"
                };
                _db.CaiDatHeThongs.Add(rowGen);
            }
            else
            {
                rowGen.GiaTriCaiDat = req.GeneralLimitsJson ?? "{}";
                rowGen.ThoiGianCapNhat = DateTime.UtcNow;
            }

            var rowDaily = await _db.CaiDatHeThongs.FirstOrDefaultAsync(x => x.KhoaCaiDat == "SHIFT_LIMITS_DAILY");
            if (rowDaily == null)
            {
                rowDaily = new Domain.Entities.CaiDatHeThong
                {
                    NhomCaiDat = "NHAN_SU",
                    KhoaCaiDat = "SHIFT_LIMITS_DAILY",
                    GiaTriCaiDat = req.DailyLimitsJson ?? "{}",
                    MoTa = "Cấu hình giới hạn ca theo ngày"
                };
                _db.CaiDatHeThongs.Add(rowDaily);
            }
            else
            {
                rowDaily.GiaTriCaiDat = req.DailyLimitsJson ?? "{}";
                rowDaily.ThoiGianCapNhat = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task DeleteShiftDefinitionAsync(int id)
        {
            var ca = await _db.CaLamViecs.FindAsync(id);
            if (ca != null)
            {
                ca.TrangThaiHoatDong = false;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<EmployeeResponse>> GetActiveEmployeesAsync()
        {
            var list = await _db.NhanViens
                .Include(x => x.VaiTro)
                .Where(x => x.TrangThaiHoatDong)
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            return list.Select(x => new EmployeeResponse
            {
                MaNhanVien = x.MaNhanVien,
                HoTen = x.HoTen,
                LuongCoBan = x.LuongCoBan ?? 25000m
            }).ToList();
        }

        public async Task<List<PhanCaResponse>> GetSchedulesAsync()
        {
            var list = await _db.PhanCaLamViecs
                .Include(x => x.NhanVien)
                .Include(x => x.Ca)
                .OrderBy(x => x.NgayLamViec)
                .ToListAsync();

            return list.Select(x => {
                var dt = x.NgayLamViec.ToDateTime(TimeOnly.MinValue);
                var thu = dt.DayOfWeek switch
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
                return new PhanCaResponse
                {
                    MaPhanCa = x.MaPhanCa,
                    MaNhanVien = x.MaNhanVien,
                    TenNhanVien = x.NhanVien.HoTen,
                    MaCa = x.MaCa,
                    TenCa = x.Ca.TenCa,
                    Gio = $"{x.Ca.GioBatDau:HH\\h}-{x.Ca.GioKetThuc:HH\\h}",
                    NgayLamViec = x.NgayLamViec.ToString("yyyy-MM-dd"),
                    ThuTrongTuan = thu,
                    GhiChu = x.GhiChu
                };
            }).ToList();
        }

        public async Task<PhanCaResponse> CreateScheduleAsync(CreatePhanCaRequest req)
        {
            if (!DateOnly.TryParse(req.NgayLamViec, out var ngay))
            {
                throw new ArgumentException("Ngày làm việc không hợp lệ (định dạng yyyy-MM-dd).");
            }

            var nv = await _db.NhanViens.FindAsync(req.MaNhanVien);
            if (nv == null) throw new InvalidOperationException("Không tìm thấy nhân viên.");

            var ca = await _db.CaLamViecs.FindAsync(req.MaCa);
            if (ca == null) throw new InvalidOperationException("Không tìm thấy ca làm việc.");

            var phanCa = new PhanCaLamViec
            {
                MaNhanVien = req.MaNhanVien,
                MaCa = req.MaCa,
                NgayLamViec = ngay,
                GhiChu = req.GhiChu
            };

            _db.PhanCaLamViecs.Add(phanCa);
            await _db.SaveChangesAsync();

            var dt = ngay.ToDateTime(TimeOnly.MinValue);
            var thu = dt.DayOfWeek switch
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

            return new PhanCaResponse
            {
                MaPhanCa = phanCa.MaPhanCa,
                MaNhanVien = phanCa.MaNhanVien,
                TenNhanVien = nv.HoTen,
                MaCa = phanCa.MaCa,
                TenCa = ca.TenCa,
                Gio = $"{ca.GioBatDau:HH\\h}-{ca.GioKetThuc:HH\\h}",
                NgayLamViec = phanCa.NgayLamViec.ToString("yyyy-MM-dd"),
                ThuTrongTuan = thu,
                GhiChu = phanCa.GhiChu
            };
        }

        public async Task DeleteScheduleAsync(int id)
        {
            var pc = await _db.PhanCaLamViecs.FindAsync(id);
            if (pc != null)
            {
                _db.PhanCaLamViecs.Remove(pc);
                await _db.SaveChangesAsync();
            }
        }

        public async Task UpdateEmployeeRateAsync(int employeeId, decimal rate)
        {
            var nv = await _db.NhanViens.FindAsync(employeeId);
            if (nv == null) throw new InvalidOperationException("Không tìm thấy nhân viên.");
            nv.LuongCoBan = rate;
            await _db.SaveChangesAsync();
        }

        public async Task<List<EmployeePayrollResponse>> GetPayrollSummaryAsync(string? ky = null)
        {
            var targetKy = string.IsNullOrWhiteSpace(ky) ? DateTime.Now.ToString("yyyy-MM") : ky;
            var parts = targetKy.Split('-');
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            if (parts.Length == 2 && int.TryParse(parts[0], out var y) && int.TryParse(parts[1], out var m))
            {
                year = y;
                month = m;
            }

            var employees = await _db.NhanViens
                .Include(x => x.VaiTro)
                .Where(x => x.TrangThaiHoatDong && x.HoTen != "Quản trị viên" && (x.VaiTro == null || x.VaiTro.TenVaiTro != "Quản trị viên"))
                .OrderBy(x => x.HoTen)
                .ToListAsync();

            var result = new List<EmployeePayrollResponse>();

            var checkIns = await _db.ChamCongs
                .Where(x => x.ThoiGianRa != null && x.ThoiGianVao.Year == year && x.ThoiGianVao.Month == month)
                .ToListAsync();

            var schedules = await _db.PhanCaLamViecs
                .Include(x => x.Ca)
                .Where(x => x.NgayLamViec.Year == year && x.NgayLamViec.Month == month)
                .ToListAsync();

            var paidBangLuongs = await _db.BangLuongs
                .Where(x => x.Ky == targetKy)
                .ToListAsync();

            var tz = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            foreach (var emp in employees)
            {
                var rate = emp.LuongCoBan ?? 25000m;
                double totalHours = 0;

                var empLogs = checkIns.Where(x => x.MaNhanVien == emp.MaNhanVien && x.ThoiGianRa.HasValue);
                foreach (var log in empLogs)
                {
                    var span = log.ThoiGianRa!.Value - log.ThoiGianVao;
                    if (span.TotalHours > 0)
                    {
                        totalHours += span.TotalHours;
                    }
                }

                if (totalHours < 0.1)
                {
                    totalHours = 0;
                    var empSchedules = schedules.Where(x => x.MaNhanVien == emp.MaNhanVien);
                    foreach (var s in empSchedules)
                    {
                        if (s.Ca != null)
                        {
                            var h = (s.Ca.GioKetThuc - s.Ca.GioBatDau).TotalHours;
                            if (h <= 0) h += 24;
                            totalHours += h;
                        }
                    }
                }

                totalHours = Math.Round(totalHours, 1);
                var totalPay = (decimal)totalHours * rate;

                var bl = paidBangLuongs.FirstOrDefault(x => x.MaNhanVien == emp.MaNhanVien);
                var isPaid = bl != null && bl.TrangThai == "DaTra";
                var timePaidStr = isPaid ? TimeZoneInfo.ConvertTimeFromUtc(bl!.ThoiGianTao, tz).ToString("dd/MM/yyyy HH:mm") : null;

                result.Add(new EmployeePayrollResponse
                {
                    MaNhanVien = emp.MaNhanVien,
                    HoTen = emp.HoTen,
                    ChucVu = emp.VaiTro?.TenVaiTro ?? "Nhân viên",
                    LuongCoBan = rate,
                    TongGioLam = totalHours,
                    TongLuong = totalPay,
                    TrangThaiThanhToan = isPaid ? "DaThanhToan" : "ChuaThanhToan",
                    ThoiGianThanhToan = timePaidStr,
                    GhiChuThanhToan = isPaid ? $"Đã thanh toán (Kỳ {targetKy})" : null
                });
            }

            return result;
        }

        public async Task PaySalaryAsync(int employeeId, int managerId, PaySalaryRequest req)
        {
            var emp = await _db.NhanViens.FindAsync(employeeId);
            if (emp == null) throw new InvalidOperationException("Không tìm thấy nhân viên.");

            var manager = await _db.NhanViens.FindAsync(managerId);
            var managerName = manager?.HoTen ?? "Quản lý";

            var kyStr = string.IsNullOrWhiteSpace(req.Ky) ? DateTime.Now.ToString("yyyy-MM") : req.Ky;

            var bl = await _db.BangLuongs
                .FirstOrDefaultAsync(x => x.MaNhanVien == employeeId && x.Ky == kyStr);

            if (bl == null)
            {
                bl = new Domain.Entities.BangLuong
                {
                    MaNhanVien = employeeId,
                    Ky = kyStr,
                    LuongTheoGio = emp.LuongCoBan ?? 25000m,
                    TrangThai = "DaTra",
                    ThoiGianTao = DateTime.UtcNow
                };
                _db.BangLuongs.Add(bl);
            }
            else
            {
                bl.TrangThai = "DaTra";
                bl.ThoiGianTao = DateTime.UtcNow;
            }

            var methodStr = string.IsNullOrWhiteSpace(req.PhuongThuc) ? "Chuyển khoản" : req.PhuongThuc;
            var noteStr = string.IsNullOrWhiteSpace(req.GhiChu) ? "" : $" (Ghi chú: {req.GhiChu})";

            _db.NhatKyHeThongs.Add(new Domain.Entities.NhatKyHeThong
            {
                MaNhanVien = managerId,
                HanhDong = "THANH TOÁN LƯƠNG",
                Module = "QUẢN LÝ LƯƠNG",
                DuLieuMoi = $"Quản lý {managerName} đã xác nhận thanh toán lương kỳ {kyStr} cho nhân viên {emp.HoTen} qua hình thức [{methodStr}].{noteStr}",
                ThietBi = "Màn hình Bảng Lương HR",
                ThoiGianTao = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
        }
    }
}
