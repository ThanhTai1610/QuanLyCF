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

    public bool ApDungKhungGio { get; set; } = false;
    public TimeSpan? GioBatDau { get; set; }
    public TimeSpan? GioKetThuc { get; set; }

    public ICollection<ChiTietCombo> ChiTiets { get; set; } = new List<ChiTietCombo>();
}
