namespace BackEnd.Domain.Entities;

/// <summary>Bảng nối n-n giữa Vai trò và Quyền (gán quyền cho vai trò).</summary>
public class VaiTroQuyen
{
    public int MaVaiTro { get; set; }
    public VaiTro VaiTro { get; set; } = null!;

    public int MaQuyen { get; set; }
    public Quyen Quyen { get; set; } = null!;
}
