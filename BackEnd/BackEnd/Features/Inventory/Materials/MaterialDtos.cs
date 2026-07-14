namespace BackEnd.Features.Inventory.Materials;

public record MaterialItem(
    int MaNguyenLieu,
    string TenNguyenLieu,
    string? MaVach_SKU,
    string PhanLoai,
    string DonViTinh,
    decimal SoLuongTon,
    decimal? MucTonToiThieu,
    decimal? GiaVonTrungBinh,
    string TrangThaiTon,
    DateTime? NgayHetHan);

public record SaveMaterialRequest(
    string TenNguyenLieu,
    string? MaVach_SKU,
    string PhanLoai,
    string DonViTinh,
    decimal? MucTonToiThieu,
    decimal? MucTonToiDa,
    int? HanSuDungNgay,
    string? HinhAnh,
    DateTime? NgayHetHan);

public record AdjustStockRequest(
    decimal SoLuongThucTe,
    string LyDo
);
