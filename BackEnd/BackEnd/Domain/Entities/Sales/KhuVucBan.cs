namespace BackEnd.Domain.Entities;

/// <summary>Khu vực đặt bàn (Tầng 1, Sân thượng, Phòng VIP...).</summary>
public class KhuVucBan
{
    public int MaKhuVuc { get; set; }
    public string TenKhuVuc { get; set; } = null!;
    public decimal PhuThu { get; set; }   // phụ thu khu vực (vd VIP), mặc định 0

    public ICollection<Ban> Bans { get; set; } = new List<Ban>();
}
