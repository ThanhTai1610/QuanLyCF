namespace BackEnd.Domain.Entities;

/// <summary>Vai trò (Role) — Quản lý, Pha chế, Thu ngân, Phục vụ, Bếp...</summary>
public class VaiTro
{
    public int MaVaiTro { get; set; }
    public string TenVaiTro { get; set; } = null!;
    public string? MoTa { get; set; }

    /// <summary>Vai trò hệ thống thì không cho xoá/sửa tên (vd: Admin).</summary>
    public bool LaVaiTroHeThong { get; set; }

    public ICollection<VaiTroQuyen> VaiTroQuyens { get; set; } = new List<VaiTroQuyen>();
    public ICollection<NhanVien> NhanViens { get; set; } = new List<NhanVien>();
}
