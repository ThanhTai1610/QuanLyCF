namespace BackEnd.Domain.Entities;

/// <summary>Combo/ưu đãi gồm nhiều sản phẩm.</summary>
public class Combo
{
    public int MaCombo { get; set; }
    public string TenCombo { get; set; } = null!;
    public decimal GiaCombo { get; set; }
    public string? HinhAnh { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;

    public ICollection<ChiTietCombo> ChiTiets { get; set; } = new List<ChiTietCombo>();
}
