namespace BackEnd.Domain.Entities;

/// <summary>Chương trình khuyến mãi / voucher.</summary>
public class KhuyenMai
{
    public int MaKhuyenMai { get; set; }
    public string? MaGiamGia { get; set; }            // mã nhập tay (unique)
    public string TenChuongTrinh { get; set; } = null!;
    public string LoaiGiamGia { get; set; } = null!;  // PhanTram, TienMat, TangMon
    public decimal GiaTriGiam { get; set; }
    public decimal? GiamToiDa { get; set; }
    public decimal? DonToiThieu { get; set; }
    public int? SoLuongGioiHan { get; set; }
    public int SoLuongDaDung { get; set; }
    public DateTime? NgayBatDau { get; set; }
    public DateTime? NgayKetThuc { get; set; }
    public string? MoTa { get; set; }
    public string? HinhAnh { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;
}

/// <summary>Đơn hàng (tại quầy / QR tại bàn).</summary>
public class DonHang
{
    public int MaDonHang { get; set; }
    public int? MaBan { get; set; }
    public Ban? Ban { get; set; }
    public int? MaKhachHang { get; set; }
    public KhachHang? KhachHang { get; set; }
    public int? MaNhanVien { get; set; }              // người tạo đơn
    public NhanVien? NhanVien { get; set; }
    public int? MaKhuyenMai { get; set; }
    public KhuyenMai? KhuyenMai { get; set; }

    public string LoaiDonHang { get; set; } = "DineIn"; // DineIn, TakeAway, QR
    public decimal TongTienHang { get; set; }
    public decimal TienGiamGia { get; set; }
    public decimal PhiDichVu { get; set; }
    public decimal ThueVAT { get; set; }
    public decimal ThanhTien { get; set; }
    public string? GhiChuDonHang { get; set; }
    public string TrangThaiDon { get; set; } = "ChoXacNhan"; // ChoXacNhan, DangPha, HoanThanh, Huy
    public string? LyDoHuy { get; set; }
    public DateTime ThoiGianTao { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }

    public ICollection<ChiTietDonHang> ChiTiets { get; set; } = new List<ChiTietDonHang>();
}

/// <summary>Dòng món trong đơn (kèm trạng thái bếp cho KDS).</summary>
public class ChiTietDonHang
{
    public int MaChiTiet { get; set; }
    public int MaDonHang { get; set; }
    public DonHang DonHang { get; set; } = null!;
    public int? MaSanPham { get; set; }
    public SanPham? SanPham { get; set; }
    public int? MaKichCo { get; set; }
    public KichCoSanPham? KichCo { get; set; }
    public int? MaCombo { get; set; }
    public Combo? Combo { get; set; }

    public int SoLuong { get; set; }
    public decimal DonGia { get; set; }               // đã cộng giá size
    public decimal TienGiamGia { get; set; }
    public decimal ThanhTien { get; set; }
    public string? GhiChuMon { get; set; }            // ít đá, 50% đường

    // Màn hình bếp (KDS)
    public string TrangThaiBep { get; set; } = "ChoLam"; // ChoLam, DangLam, HoanThanh, DaTraKhach
    public int? MaNhanVienPhaChe { get; set; }
    public NhanVien? NhanVienPhaChe { get; set; }
    public bool BaoHetNguyenLieu { get; set; }
    public DateTime? ThoiGianBaoBep { get; set; }
    public DateTime? ThoiGianLamXong { get; set; }
}

/// <summary>Hoá đơn (sinh khi thanh toán đơn).</summary>
public class HoaDon
{
    public int MaHoaDon { get; set; }
    public int MaDonHang { get; set; }                // unique
    public DonHang DonHang { get; set; } = null!;
    public int? MaNhanVienThuNgan { get; set; }
    public NhanVien? NhanVienThuNgan { get; set; }
    public string? MaSoThueXuatHD { get; set; }
    public decimal TongThanhTien { get; set; }
    public decimal SoTienKhachTra { get; set; }
    public decimal TienThoiLai { get; set; }
    public string TrangThai { get; set; } = "DaThanhToan"; // ChuaTT, ThanhToanMotPhan, DaThanhToan
    public DateTime ThoiGianThanhToan { get; set; }

    public ICollection<ThanhToanChiTiet> ChiTietThanhToans { get; set; } = new List<ThanhToanChiTiet>();
}

/// <summary>Chi tiết thanh toán (nhiều phương thức / nhiều lần).</summary>
public class ThanhToanChiTiet
{
    public int MaThanhToan { get; set; }
    public int MaHoaDon { get; set; }
    public HoaDon HoaDon { get; set; } = null!;
    public string PhuongThuc { get; set; } = null!;   // TienMat, ChuyenKhoan, Momo, VNPay, ZaloPay, The
    public decimal SoTien { get; set; }
    public string? MaGiaoDichCong { get; set; }
    public DateTime ThoiGianThanhToan { get; set; }
}
