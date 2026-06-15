namespace BackEnd.Domain.Entities;

/// <summary>Phiếu kho: Nhập kho / Kiểm kê / Xuất huỷ / Trả hàng.</summary>
public class PhieuKho
{
    public int MaPhieu { get; set; }
    public string LoaiPhieu { get; set; } = null!;        // NhapKho, KiemKe, XuatHuy, TraHang

    public int MaNhanVien { get; set; }
    public NhanVien NhanVien { get; set; } = null!;

    public int? MaNhaCungCap { get; set; }
    public NhaCungCap? NhaCungCap { get; set; }

    public decimal? TongTienHang { get; set; }
    public decimal? TienDaThanhToan { get; set; }
    public string? TrangThaiThanhToan { get; set; }       // ChuaThanhToan, ThanhToanMotPhan, DaThanhToan

    /// <summary>Trạng thái phiếu (dùng cho kiểm kê): ChoDuyet, DaDuyet, TuChoi, DaHoanTat.</summary>
    public string TrangThai { get; set; } = "DaHoanTat";

    public string? GhiChu { get; set; }
    public DateTime ThoiGianTao { get; set; }

    public ICollection<ChiTietPhieuKho> ChiTiets { get; set; } = new List<ChiTietPhieuKho>();
}
