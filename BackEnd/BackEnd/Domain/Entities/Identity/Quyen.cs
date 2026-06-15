namespace BackEnd.Domain.Entities;

/// <summary>Quyền (Permission) đơn lẻ — vd: DONHANG_XEM, THANHTOAN, CAIDAT.</summary>
public class Quyen
{
    public int MaQuyen { get; set; }

    /// <summary>Mã quyền duy nhất, dùng trong code/policy (vd: "TAIKHOAN_SUA").</summary>
    public string MaCode { get; set; } = null!;

    public string TenQuyen { get; set; } = null!;

    /// <summary>Nhóm chức năng để hiển thị: TaiKhoan, SanPham, Kho, DonHang...</summary>
    public string Nhom { get; set; } = null!;

    public ICollection<VaiTroQuyen> VaiTroQuyens { get; set; } = new List<VaiTroQuyen>();
}
