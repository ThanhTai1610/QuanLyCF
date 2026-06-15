namespace BackEnd.Domain.Entities;

/// <summary>Danh mục sản phẩm (hỗ trợ đa cấp qua MaDanhMucCha).</summary>
public class DanhMuc
{
    public int MaDanhMuc { get; set; }
    public int? MaDanhMucCha { get; set; }
    public DanhMuc? DanhMucCha { get; set; }

    public string TenDanhMuc { get; set; } = null!;
    public string? HinhAnh { get; set; }
    public int ThuTuHienThi { get; set; }
    public string? MoTa { get; set; }
    public bool TrangThaiHoatDong { get; set; } = true;

    public ICollection<DanhMuc> DanhMucCon { get; set; } = new List<DanhMuc>();
    public ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
