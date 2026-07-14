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
    }

    public class ReviewRequestPayload
    {
        public string Status { get; set; } = null!; // "DaDuyet" or "TuChoi"
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
    }
}
