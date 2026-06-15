namespace BackEnd.Features.Inventory.Suppliers;

public record SupplierItem(
    int MaNhaCungCap,
    string TenNhaCungCap,
    string? NguoiLienHe,
    string? SoDienThoai,
    string? Email,
    decimal CongNoHienTai);

public record SaveSupplierRequest(
    string TenNhaCungCap,
    string? MaSoThue,
    string? NguoiLienHe,
    string? SoDienThoai,
    string? Email,
    string? DiaChi,
    string? SoTaiKhoan,
    string? TenNganHang);

/// <summary>Phiếu chi trả nợ cho NCC.</summary>
public record PaySupplierRequest(decimal SoTien);
