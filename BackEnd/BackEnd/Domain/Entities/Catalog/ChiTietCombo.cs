namespace BackEnd.Domain.Entities;

/// <summary>Dòng chi tiết: sản phẩm nào, số lượng bao nhiêu trong combo.</summary>
public class ChiTietCombo
{
    public int MaChiTietCombo { get; set; }

    public int MaCombo { get; set; }
    public Combo Combo { get; set; } = null!;

    public int MaSanPham { get; set; }
    public SanPham SanPham { get; set; } = null!;

    public int SoLuong { get; set; } = 1;
}
