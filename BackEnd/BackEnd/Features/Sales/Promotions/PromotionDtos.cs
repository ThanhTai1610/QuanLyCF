namespace BackEnd.Features.Sales.Promotions;

// Hiển thị khuyến mãi
public record PromotionItem(
    int MaKhuyenMai,
    string? MaGiamGia,
    string TenChuongTrinh,
    string LoaiGiamGia,           // PhanTram | TienMat
    decimal GiaTriGiam,
    decimal? GiamToiDa,
    decimal? DonToiThieu,
    int? SoLuongGioiHan,
    int SoLuongDaDung,
    DateTime? NgayBatDau,
    DateTime? NgayKetThuc,
    string? MoTa,
    bool TrangThaiHoatDong);

// Tạo / cập nhật
public record SavePromotionRequest(
    string? MaGiamGia,
    string TenChuongTrinh,
    string LoaiGiamGia,
    decimal GiaTriGiam,
    decimal? GiamToiDa,
    decimal? DonToiThieu,
    int? SoLuongGioiHan,
    DateTime? NgayBatDau,
    DateTime? NgayKetThuc,
    string? MoTa,
    bool TrangThaiHoatDong);

// Áp dụng (xem trước) — theo mã nhập tay HOẶC chọn chương trình
public record ApplyPromotionRequest(int? MaKhuyenMai, string? Code, decimal TongTien);
public record ApplyPromotionResult(int MaKhuyenMai, string TenChuongTrinh, decimal TienGiam, decimal ThanhTienSau);
