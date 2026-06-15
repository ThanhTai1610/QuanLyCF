namespace BackEnd.Domain.Entities;

/// <summary>Định nghĩa ca làm.</summary>
public class CaLamViec
{
    public int MaCa { get; set; }
    public string TenCa { get; set; } = null!;
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;
}

/// <summary>Xếp ca cho nhân viên theo ngày.</summary>
public class PhanCaLamViec
{
    public int MaPhanCa { get; set; }
    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;
    public int MaCa { get; set; }
    public CaLamViec Ca { get; set; } = null!;
    public DateOnly NgayLamViec { get; set; }
    public string? GhiChu { get; set; }
}

/// <summary>Chấm công (kèm ảnh FaceID).</summary>
public class ChamCong
{
    public int MaChamCong { get; set; }
    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;
    public int? MaCa { get; set; }
    public CaLamViec? Ca { get; set; }
    public DateTime ThoiGianVao { get; set; }
    public DateTime? ThoiGianRa { get; set; }
    public string? AnhVao { get; set; }
    public string? AnhRa { get; set; }
    public int SoPhutDiTre { get; set; }
    public int SoPhutVeSom { get; set; }
    public int LamThemGioPhut { get; set; }
    public string? TrangThai { get; set; }            // HopLe, KhongHopLe, XinPhep, ChoDuyet
    public string? GhiChu { get; set; }
}

/// <summary>Đơn từ nhân viên (nghỉ phép, tăng ca...).</summary>
public class DonTuNhanVien
{
    public int MaDon { get; set; }
    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;
    public string LoaiDon { get; set; } = null!;      // PhepNam, TangCa, NghiKhongLuong, NghiBu
    public string? ThoiGianLienQuan { get; set; }
    public string? LyDo { get; set; }
    public string TrangThai { get; set; } = "ChoDuyet"; // ChoDuyet, DaDuyet, TuChoi
    public int? MaNguoiDuyet { get; set; }
    public DateTime ThoiGianTao { get; set; }
}

/// <summary>Bảng lương theo kỳ.</summary>
public class BangLuong
{
    public int MaBangLuong { get; set; }
    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;
    public string Ky { get; set; } = null!;           // 2026-06
    public decimal LuongTheoGio { get; set; }
    public decimal SoGioThuong { get; set; }
    public decimal SoGioOT { get; set; }
    public decimal SoNgayPhep { get; set; }
    public decimal PhuCap { get; set; }
    public decimal Thuong { get; set; }
    public decimal Phat { get; set; }
    public decimal ThucLanh { get; set; }
    public string TrangThai { get; set; } = "Nhap";   // Nhap, DaChot, DaTra
    public DateTime ThoiGianTao { get; set; }
}
