namespace BackEnd.Domain.Entities;

/// <summary>Nguyên liệu/vật tư tồn kho.</summary>
public class NguyenLieu
{
    public int MaNguyenLieu { get; set; }
    public string TenNguyenLieu { get; set; } = null!;
    public string? MaVach_SKU { get; set; }
    public string DonViTinh { get; set; } = null!;        // g, ml, Lon, Chiếc...
    public decimal SoLuongTon { get; set; }
    public decimal? MucTonToiThieu { get; set; }
    public decimal? MucTonToiDa { get; set; }
    public decimal? GiaVonTrungBinh { get; set; }         // bình quân gia quyền
    public int? HanSuDungNgay { get; set; }
    public string? HinhAnh { get; set; }

    public ICollection<ChiTietPhieuKho> ChiTietPhieuKhos { get; set; } = new List<ChiTietPhieuKho>();
}
