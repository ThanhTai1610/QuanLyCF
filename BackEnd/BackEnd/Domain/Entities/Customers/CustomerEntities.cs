namespace BackEnd.Domain.Entities;

/// <summary>Khách hàng thân thiết.</summary>
public class KhachHang
{
    public int MaKhachHang { get; set; }
    public string? Email { get; set; }                // unique (khi khác null)
    public string? HoTen { get; set; }
    public string? SoDienThoai { get; set; }
    public string? GioiTinh { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public string? AnhDaiDien { get; set; }
    public string HangThanhVien { get; set; } = "Member"; // Member, Silver, Gold, Diamond
    public int DiemTichLuy { get; set; }
    public decimal TongTienDaTieu { get; set; }
    public DateTime? LanGheThamCuoi { get; set; }
    public string? GhiChuKhachHang { get; set; }
    public DateTime ThoiGianTao { get; set; }
}

/// <summary>Phần thưởng đổi điểm.</summary>
public class PhanThuong
{
    public int MaPhanThuong { get; set; }
    public string TenPhanThuong { get; set; } = null!;
    public int DiemCanDoi { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;
}

/// <summary>Lịch sử biến động điểm.</summary>
public class LichSuDiem
{
    public int MaLichSuDiem { get; set; }
    public int MaKhachHang { get; set; }
    public KhachHang KhachHang { get; set; } = null!;
    public int? MaDonHang { get; set; }
    public DonHang? DonHang { get; set; }
    public string LoaiBienDong { get; set; } = null!; // Tich, Doi, DieuChinh
    public int SoDiem { get; set; }                   // + tích / - đổi
    public string? GhiChu { get; set; }
    public DateTime ThoiGianTao { get; set; }
}

/// <summary>Đánh giá của khách.</summary>
public class DanhGia
{
    public int MaDanhGia { get; set; }
    public int MaKhachHang { get; set; }
    public KhachHang KhachHang { get; set; } = null!;
    public int? MaDonHang { get; set; }
    public DonHang? DonHang { get; set; }
    public int DiemSo { get; set; }                   // 1..5
    public string? BinhLuan { get; set; }
    public string? HinhAnhDinhKem { get; set; }
    public string? PhanHoiTuQuanLy { get; set; }
    public bool TrangThaiHienThi { get; set; } = true;
    public DateTime ThoiGianTao { get; set; }
}

/// <summary>Lịch sử hội thoại chatbot.</summary>
public class LichSuChatbot
{
    public int MaLichSu { get; set; }
    public int? MaKhachHang { get; set; }
    public KhachHang? KhachHang { get; set; }
    public string? PhienChat { get; set; }
    public string TinNhanKhach { get; set; } = null!;
    public string PhanHoiBot { get; set; } = null!;
    public string? IntentContext { get; set; }        // HoiMenu, KhieuNai, HoiKhuyenMai
    public DateTime ThoiGianTao { get; set; }
}
