namespace BackEnd.Domain.Entities;

/// <summary>Kích cỡ (size) của sản phẩm và giá cộng thêm.</summary>
public class KichCoSanPham
{
    public int MaKichCo { get; set; }

    public int MaSanPham { get; set; }
    public SanPham SanPham { get; set; } = null!;

    public string TenKichCo { get; set; } = null!;   // Size M, Size L
    public decimal GiaCongThem { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;
}
