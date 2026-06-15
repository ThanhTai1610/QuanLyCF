namespace BackEnd.Domain.Entities;

/// <summary>Bàn trong quán, gắn mã QR để khách quét đặt món.</summary>
public class Ban
{
    public int MaBan { get; set; }

    public int MaKhuVuc { get; set; }
    public KhuVucBan KhuVuc { get; set; } = null!;

    public string TenBan { get; set; } = null!;       // "Bàn 5" (unique)
    public int? SucChua { get; set; }                  // số ghế
    public string MaQRHash { get; set; } = null!;      // token QR (unique) → URL đặt món
    public string TrangThai { get; set; } = "Trong";   // Trong, CoKhach, BaoTri
}
