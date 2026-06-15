namespace BackEnd.Domain.Entities;

/// <summary>Dòng chi tiết phiếu kho.</summary>
public class ChiTietPhieuKho
{
    public int MaChiTietPhieu { get; set; }

    public int MaPhieu { get; set; }
    public PhieuKho PhieuKho { get; set; } = null!;

    public int MaNguyenLieu { get; set; }
    public NguyenLieu NguyenLieu { get; set; } = null!;

    public decimal SoLuong { get; set; }                  // SL nhập/xuất (hoặc tồn hệ thống lúc kiểm kê)
    public decimal? DonGia { get; set; }                  // giá lúc nhập
    public decimal? SoLuongThucTe { get; set; }           // số đếm thực tế khi kiểm kê
    public string? LyDoLech { get; set; }
}
