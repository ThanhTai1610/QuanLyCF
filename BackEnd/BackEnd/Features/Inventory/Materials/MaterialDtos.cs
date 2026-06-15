namespace BackEnd.Features.Inventory.Materials;

public record MaterialItem(
    int MaNguyenLieu,
    string TenNguyenLieu,
    string? MaVach_SKU,
    string DonViTinh,
    decimal SoLuongTon,
    decimal? MucTonToiThieu,
    decimal? GiaVonTrungBinh,
    string TrangThaiTon);   // Ok, SapHet, Het

public record SaveMaterialRequest(
    string TenNguyenLieu,
    string? MaVach_SKU,
    string DonViTinh,
    decimal? MucTonToiThieu,
    decimal? MucTonToiDa,
    int? HanSuDungNgay,
    string? HinhAnh);
