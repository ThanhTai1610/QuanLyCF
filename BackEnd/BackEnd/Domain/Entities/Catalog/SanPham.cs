namespace BackEnd.Domain.Entities;

/// <summary>Sản phẩm trên thực đơn (đồ uống/món/topping).</summary>
public class SanPham
{
    public int MaSanPham { get; set; }

    public int? MaDanhMuc { get; set; }
    public DanhMuc? DanhMuc { get; set; }

    public string TenSanPham { get; set; } = null!;
    public string? MaVach_SKU { get; set; }
    public decimal? GiaVonDuKien { get; set; }   // nhập thủ công (đã bỏ định mức tự động)
    public decimal GiaBan { get; set; }
    public string? HinhAnh { get; set; }
    public string? MoTa { get; set; }
    public int? LuongCalo { get; set; }
    public int? ThoiGianChuanBiPhut { get; set; }
    public bool LaMonNoiBat { get; set; }
    public string KieuMon { get; set; } = "MonChinh"; // MonChinh, Topping, MonKem
    public bool TrangThaiBan { get; set; } = true;

    public ICollection<KichCoSanPham> KichCos { get; set; } = new List<KichCoSanPham>();
}
