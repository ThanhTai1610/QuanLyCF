namespace BackEnd.Domain.Entities;

/// <summary>Nhật ký hệ thống (audit log).</summary>
public class NhatKyHeThong
{
    public int MaNhatKy { get; set; }
    public int? MaNhanVien { get; set; }
    public NhanVien? NhanVien { get; set; }
    public string HanhDong { get; set; } = null!;       // CREATE, UPDATE, DELETE, LOGIN
    public string Module { get; set; } = null!;
    public string? DuLieuCu { get; set; }               // JSON trước khi sửa
    public string? DuLieuMoi { get; set; }              // JSON sau khi sửa
    public string? DiaChiIP { get; set; }
    public string? ThietBi { get; set; }
    public DateTime ThoiGianTao { get; set; }
}

/// <summary>Cài đặt hệ thống (key-value theo nhóm).</summary>
public class CaiDatHeThong
{
    public int MaCaiDat { get; set; }
    public string NhomCaiDat { get; set; } = null!;     // CHUNG, THANH_TOAN, TICH_DIEM
    public string KhoaCaiDat { get; set; } = null!;     // THUE_VAT_MAC_DINH... (unique)
    public string? GiaTriCaiDat { get; set; }
    public string? MoTa { get; set; }
    public DateTime ThoiGianCapNhat { get; set; }
}
