namespace BackEnd.Features.System
{
    // ── Cài đặt hệ thống ──────────────────────────────────────────────────────

    /// <summary>Trả về toàn bộ cài đặt hệ thống (chỉ cho Admin).</summary>
    public record SystemSettingsDto(
        string TenQuan,
        string DiaChi,
        string SoDienThoai,
        string MoTaQuan,
        string ThueVatMacDinh,
        string PhiDichVu,
        string TyLeTichDiem,
        bool   CheDoBaoTri,
        string ThongDiepBaoTri
    );

    /// <summary>Request cập nhật cài đặt hệ thống.</summary>
    public record UpdateSettingsRequest(
        string TenQuan,
        string DiaChi,
        string SoDienThoai,
        string MoTaQuan,
        string ThueVatMacDinh,
        string PhiDichVu,
        string TyLeTichDiem,
        bool   CheDoBaoTri,
        string ThongDiepBaoTri
    );

    /// <summary>Thông tin công khai của quán (không cần đăng nhập).</summary>
    public record StoreInfoDto(
        string TenQuan,
        string DiaChi,
        string SoDienThoai,
        string MoTaQuan
    );

    /// <summary>Trạng thái bảo trì — endpoint công khai.</summary>
    public record MaintenanceStatusDto(
        bool   IsMaintenance,
        string Message
    );

    // ── Nhật ký hệ thống ──────────────────────────────────────────────────────

    /// <summary>Một mục trong nhật ký hệ thống.</summary>
    public record AuditLogItem(
        int     MaNhatKy,
        int?    MaNhanVien,
        string? TenNhanVien,
        string  HanhDong,
        string  Module,
        string? DuLieuCu,
        string? DuLieuMoi,
        string? DiaChiIP,
        string? ThietBi,
        DateTime ThoiGianTao
    );

    /// <summary>Tham số truy vấn nhật ký.</summary>
    public record AuditLogQuery(
        string? Module    = null,
        string? HanhDong  = null,
        int?    MaNhanVien = null,
        int     Page      = 1,
        int     PageSize  = 20
    );
}
