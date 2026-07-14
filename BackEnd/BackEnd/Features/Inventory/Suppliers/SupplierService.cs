using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Inventory.Suppliers;

public class SupplierService
{
    private readonly QuanLyCFDbContext _db;
    public SupplierService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<SupplierItem>> LayDanhSachAsync(string? q)
    {
        var query = _db.NhaCungCaps.AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
        {
            var kw = q.Trim().ToLower();
            query = query.Where(x => x.TenNhaCungCap.ToLower().Contains(kw)
                                  || (x.SoDienThoai != null && x.SoDienThoai.Contains(kw)));
        }
        return await query.OrderBy(x => x.TenNhaCungCap)
            .Select(x => new SupplierItem(x.MaNhaCungCap, x.TenNhaCungCap, x.NguoiLienHe, x.SoDienThoai, x.Email, x.CongNoHienTai))
            .ToListAsync();
    }

    public async Task<ServiceResult<int>> TaoAsync(SaveSupplierRequest req)
    {
        var ncc = new NhaCungCap
        {
            TenNhaCungCap = req.TenNhaCungCap.Trim(),
            MaSoThue = req.MaSoThue,
            NguoiLienHe = req.NguoiLienHe,
            SoDienThoai = req.SoDienThoai,
            Email = req.Email,
            DiaChi = req.DiaChi,
            SoTaiKhoan = req.SoTaiKhoan,
            TenNganHang = req.TenNganHang,
        };
        _db.NhaCungCaps.Add(ncc);
        await _db.SaveChangesAsync();
        return ServiceResult<int>.Ok(ncc.MaNhaCungCap);
    }

    public async Task<ServiceResult<bool>> CapNhatAsync(int id, SaveSupplierRequest req)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return ServiceResult<bool>.Fail("Không tìm thấy nhà cung cấp.");
        
        ncc.TenNhaCungCap = req.TenNhaCungCap.Trim();
        ncc.MaSoThue = req.MaSoThue;
        ncc.NguoiLienHe = req.NguoiLienHe;
        ncc.SoDienThoai = req.SoDienThoai;
        ncc.Email = req.Email;
        ncc.DiaChi = req.DiaChi;
        ncc.SoTaiKhoan = req.SoTaiKhoan;
        ncc.TenNganHang = req.TenNganHang;
        
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<decimal>> TraCongNoAsync(int id, decimal soTien, string phuongThucThanhToan, int maNhanVien)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return ServiceResult<decimal>.Fail("Không tìm thấy nhà cung cấp.");
        if (soTien <= 0) return ServiceResult<decimal>.Fail("Số tiền phải lớn hơn 0.");

        ncc.CongNoHienTai = Math.Max(0, ncc.CongNoHienTai - soTien);

        _db.DongTiens.Add(new DongTien
        {
            LoaiGiaoDich = "Chi",
            NhomGiaoDich = "TraNoNCC",
            PhuongThucThanhToan = phuongThucThanhToan,
            SoTien = soTien,
            NguoiNopNhan = ncc.TenNhaCungCap,
            GhiChu = $"Trả nợ nhà cung cấp {ncc.TenNhaCungCap}",
            MaNhanVienGhiNhan = maNhanVien,
            ThoiGianTao = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return ServiceResult<decimal>.Ok(ncc.CongNoHienTai);
    }

    public async Task<ServiceResult<bool>> XoaAsync(int id)
    {
        var ncc = await _db.NhaCungCaps.FindAsync(id);
        if (ncc is null) return ServiceResult<bool>.Fail("Không tìm thấy nhà cung cấp.");
        if (await _db.PhieuKhos.AnyAsync(x => x.MaNhaCungCap == id))
            return ServiceResult<bool>.Fail("Nhà cung cấp đã có phiếu nhập, không thể xoá.");
            
        _db.NhaCungCaps.Remove(ncc);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
