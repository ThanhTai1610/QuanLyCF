namespace BackEnd.Domain.Entities;

/// <summary>Dòng tiền thu/chi (sổ cái thu chi nội bộ).</summary>
public class DongTien
{
    public int MaDongTien { get; set; }
    public string LoaiGiaoDich { get; set; } = null!;   // Thu, Chi
    public string NhomGiaoDich { get; set; } = null!;   // DoanhThuPOS, NhapHang, TraLuong, DienNuoc, MatBang
    public int? MaThamChieu { get; set; }               // ID HoaDon / PhieuKho liên quan
    public string PhuongThucThanhToan { get; set; } = null!;
    public decimal SoTien { get; set; }
    public string? NguoiNopNhan { get; set; }
    public string? GhiChu { get; set; }
    public string? ChungTuDinhKem { get; set; }
    public int MaNhanVienGhiNhan { get; set; }
    public NhanVien NhanVienGhiNhan { get; set; } = null!;
    public DateTime ThoiGianTao { get; set; }
}
