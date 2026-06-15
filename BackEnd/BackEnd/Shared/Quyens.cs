namespace BackEnd.Shared;

/// <summary>Danh mục mã quyền (permission code) dùng cho RBAC và [Authorize(Policy = ...)].</summary>
public static class Quyens
{
    public const string TaiKhoanXem      = "TAIKHOAN_XEM";
    public const string TaiKhoanQuanLy   = "TAIKHOAN_QUANLY";
    public const string SanPhamXem       = "SANPHAM_XEM";
    public const string SanPhamQuanLy    = "SANPHAM_QUANLY";
    public const string KhoXem           = "KHO_XEM";
    public const string KhoQuanLy        = "KHO_QUANLY";
    public const string DonHangXem       = "DONHANG_XEM";
    public const string DonHangXuLy      = "DONHANG_XULY";
    public const string BepXem           = "BEP_XEM";
    public const string ThanhToan        = "THANHTOAN";
    public const string KhachHangXem     = "KHACHHANG_XEM";
    public const string KhachHangQuanLy  = "KHACHHANG_QUANLY";
    public const string NhanSuXem        = "NHANSU_XEM";
    public const string NhanSuQuanLy     = "NHANSU_QUANLY";
    public const string BaoCaoXem        = "BAOCAO_XEM";
    public const string CaiDatQuanLy     = "CAIDAT_QUANLY";

    /// <summary>Toàn bộ mã quyền — dùng để cấp full quyền cho vai trò Quản lý.</summary>
    public static readonly string[] TatCa =
    {
        TaiKhoanXem, TaiKhoanQuanLy, SanPhamXem, SanPhamQuanLy, KhoXem, KhoQuanLy,
        DonHangXem, DonHangXuLy, BepXem, ThanhToan, KhachHangXem, KhachHangQuanLy,
        NhanSuXem, NhanSuQuanLy, BaoCaoXem, CaiDatQuanLy
    };
}
