using System.ComponentModel.DataAnnotations;

namespace BackEnd.Features.Hr
{
    public class CheckInRequest
    {
        [Required]
        public string Type { get; set; } = null!; // "in" or "out"

        public int? MaCa { get; set; }

        public string? PhotoUrl { get; set; }

        public string? GhiChu { get; set; }

        public int? MaNhanVien { get; set; }
    }

    public class CreateRequest
    {
        [Required]
        public string LoaiDon { get; set; } = null!; // PhepNam, TangCa, NghiKhongLuong, NghiBu

        [Required]
        public string ThoiGianLienQuan { get; set; } = null!; // e.g. "T5, 20/04 (Ca Sáng)"

        [Required]
        public string LyDo { get; set; } = null!;

        public int? MaNhanVien { get; set; }
    }

    public class ForceCheckOutRequest
    {
        public string? Reason { get; set; }
    }

    public class ChamCongResponse
    {
        public int MaChamCong { get; set; }
        public int? MaCa { get; set; }
        public string? TenCa { get; set; }
        public string Date { get; set; } = null!;
        public string? TimeIn { get; set; }
        public string? TimeOut { get; set; }
        public string? ImgIn { get; set; }
        public string? ImgOut { get; set; }
        public string? TimeInExact { get; set; }
        public string? TimeOutExact { get; set; }
        public string? Total { get; set; }
        public string? GhiChu { get; set; }
        public string TrangThai { get; set; } = "ChoDuyet"; // ChoDuyet, DaDuyet, TuChoi, HopLe, KhongHopLe
    }

    public class ReviewCheckInPayload
    {
        public string Status { get; set; } = "DaDuyet"; // DaDuyet, TuChoi
        public string? Note { get; set; }
    }

    public class DonTuResponse
    {
        public int MaDon { get; set; }
        public string LoaiDon { get; set; } = null!;
        public string ThoiGianLienQuan { get; set; } = null!;
        public string LyDo { get; set; } = null!;
        public string TrangThai { get; set; } = null!; // ChoDuyet, DaDuyet, TuChoi
        public string ThoiGianTao { get; set; } = null!;
        public string? TenNhanVien { get; set; }
        public string? GhiChuDuyet { get; set; }
    }

    public class ReviewRequestPayload
    {
        public string Status { get; set; } = null!; // "DaDuyet" or "TuChoi"
        public string? Note { get; set; } // Lý do từ chối đơn
    }

    public class SaveShiftLimitsRequest
    {
        public string GeneralLimitsJson { get; set; } = "{}";
        public string DailyLimitsJson { get; set; } = "{}";
    }

    public class ShiftLimitsResponse
    {
        public string GeneralLimitsJson { get; set; } = "{}";
        public string DailyLimitsJson { get; set; } = "{}";
    }

    public class SaveShiftDefinitionRequest
    {
        [Required]
        public string TenCa { get; set; } = null!;
        [Required]
        public string GioBatDau { get; set; } = null!; // "HH:mm"
        [Required]
        public string GioKetThuc { get; set; } = null!; // "HH:mm"
    }

    public class CaLamViecResponse
    {
        public int MaCa { get; set; }
        public string TenCa { get; set; } = null!;
        public string GioBatDau { get; set; } = null!;
        public string GioKetThuc { get; set; } = null!;
    }

    public class EmployeeResponse
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = null!;
        public decimal LuongCoBan { get; set; }
    }

    public class PhanCaResponse
    {
        public int MaPhanCa { get; set; }
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = null!;
        public int MaCa { get; set; }
        public string TenCa { get; set; } = null!;
        public string Gio { get; set; } = null!;
        public string NgayLamViec { get; set; } = null!; // ISO string yyyy-MM-dd
        public string ThuTrongTuan { get; set; } = null!; // T2, T3, ...
        public string? GhiChu { get; set; }
    }

    public class CreatePhanCaRequest
    {
        [Required]
        public int MaNhanVien { get; set; }
        [Required]
        public int MaCa { get; set; }
        [Required]
        public string NgayLamViec { get; set; } = null!; // "yyyy-MM-dd"
        public string? GhiChu { get; set; }
    }

    public class EmployeePayrollResponse
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; } = null!;
        public string ChucVu { get; set; } = null!;
        public decimal LuongCoBan { get; set; }
        public double TongGioLam { get; set; }
        public decimal TongLuong { get; set; }
        public string TrangThaiThanhToan { get; set; } = "ChuaThanhToan"; // ChuaThanhToan, DaThanhToan
        public string? ThoiGianThanhToan { get; set; }
        public string? GhiChuThanhToan { get; set; }
    }

    public class PaySalaryRequest
    {
        public string Ky { get; set; } = DateTime.Now.ToString("yyyy-MM");
        public string? PhuongThuc { get; set; } = "Chuyển khoản";
        public string? GhiChu { get; set; }
    }

    public class UpdateEmployeeRateRequest
    {
        [Required]
        public decimal LuongCoBan { get; set; }
    }
}
