namespace BackEnd.Domain.Entities;

/// <summary>Tài khoản nhân viên (đăng nhập hệ thống).</summary>
public class NhanVien
{
    public int MaNhanVien { get; set; }

    public int MaVaiTro { get; set; }
    public VaiTro VaiTro { get; set; } = null!;

    public string HoTen { get; set; } = null!;
    public string Email { get; set; } = null!;            // dùng làm tên đăng nhập
    public string MatKhauHash { get; set; } = null!;       // BCrypt
    public string? MaPinHash { get; set; }                 // PIN đăng nhập ca (StaffLogin), BCrypt

    public string? TokenKhoiPhucMatKhau { get; set; }
    public DateTime? HanTokenKhoiPhuc { get; set; }

    public string? SoDienThoai { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public string? DiaChi { get; set; }
    public string? SoCCCD { get; set; }
    public string? SoTaiKhoanNganHang { get; set; }
    public string? TenNganHang { get; set; }
    public decimal? LuongCoBan { get; set; }
    public string? AnhDaiDien { get; set; }

    public bool TrangThaiHoatDong { get; set; } = true;    // true = hoạt động, false = đã khoá

    // Bảo mật đăng nhập
    public DateTime? LanDangNhapCuoi { get; set; }
    public int SoLanDangNhapSai { get; set; }
    public DateTime? KhoaDenKhi { get; set; }              // khoá tạm sau nhiều lần sai

    public DateTime ThoiGianTao { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
