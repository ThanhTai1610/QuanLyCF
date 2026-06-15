namespace BackEnd.Domain.Entities;

/// <summary>Nhà cung cấp nguyên liệu.</summary>
public class NhaCungCap
{
    public int MaNhaCungCap { get; set; }
    public string TenNhaCungCap { get; set; } = null!;
    public string? MaSoThue { get; set; }
    public string? NguoiLienHe { get; set; }
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public string? DiaChi { get; set; }
    public string? SoTaiKhoan { get; set; }
    public string? TenNganHang { get; set; }
    public decimal CongNoHienTai { get; set; }            // tiền đang nợ NCC

    public ICollection<PhieuKho> PhieuKhos { get; set; } = new List<PhieuKho>();
}
