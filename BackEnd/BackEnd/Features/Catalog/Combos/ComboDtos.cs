namespace BackEnd.Features.Catalog.Combos;

public record ComboLineDto(int MaSanPham, int SoLuong, string? TenSanPham = null);

public record ComboListItem(
    int MaCombo,
    string TenCombo,
    decimal GiaCombo,
    string? HinhAnh,
    bool TrangThaiHoatDong,
    int SoMon);

public record ComboDetail(
    int MaCombo,
    string TenCombo,
    decimal GiaCombo,
    string? HinhAnh,
    string? MoTa,
    bool TrangThaiHoatDong,
    IEnumerable<ComboLineDto> ChiTiets);

public record SaveComboRequest(
    string TenCombo,
    decimal GiaCombo,
    string? HinhAnh,
    string? MoTa,
    bool TrangThaiHoatDong,
    List<ComboLineDto> ChiTiets);
