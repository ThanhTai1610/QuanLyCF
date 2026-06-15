namespace BackEnd.Features.Sales.Tables;

// ── Khu vực ─────────────────────────────────────────────────────
public record ZoneItem(int MaKhuVuc, string TenKhuVuc, decimal PhuThu, int SoBan);
public record SaveZoneRequest(string TenKhuVuc, decimal PhuThu);

// ── Bàn ─────────────────────────────────────────────────────────
public record TableItem(
    int MaBan,
    string TenBan,
    int MaKhuVuc,
    string TenKhuVuc,
    int? SucChua,
    string TrangThai,
    string MaQRHash,
    string UrlDatMon);

public record SaveTableRequest(string TenBan, int MaKhuVuc, int? SucChua);
public record UpdateTableStatusRequest(string TrangThai);   // Trong, CoKhach, BaoTri
