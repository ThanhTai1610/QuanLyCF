namespace BackEnd.Features.Finance.CashFlows;

public record CashFlowListItem(
    int MaDongTien,
    string LoaiGiaoDich,
    string NhomGiaoDich,
    decimal SoTien,
    string PhuongThucThanhToan,
    string? NguoiNopNhan,
    string? GhiChu,
    DateTime ThoiGianTao,
    string NguoiGhiNhan
);

public record CreateCashOutRequest(
    string NhomGiaoDich,
    string PhuongThucThanhToan,
    decimal SoTien,
    string? NguoiNopNhan,
    string GhiChu
);

public record CashFlowSummary(
    decimal TongThu,
    decimal TongChi,
    decimal DongTienThuan,
    decimal ChiLuong,
    decimal ChiKho,
    decimal ChiKhac
);

public record SalaryListItem(
    int MaBangLuong,
    string HoTen,
    string TenVaiTro,
    decimal LuongTheoGio,
    decimal SoGioThuong,
    decimal SoGioOT,
    decimal PhuCap,
    decimal Thuong,
    decimal Phat,
    decimal ThucLanh,
    string TrangThai
);
