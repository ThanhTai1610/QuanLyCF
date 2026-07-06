using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Catalog.Categories;

public class CategoryService
{
    private readonly QuanLyCFDbContext _db;
    public CategoryService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<CategoryItem>> ListAsync(bool? hienThi)
    {
        var query = _db.DanhMucs.AsQueryable();
        if (hienThi is { } h) query = query.Where(x => x.TrangThaiHoatDong == h);

        return await query.OrderBy(x => x.ThuTuHienThi).ThenBy(x => x.TenDanhMuc)
            .Select(x => new CategoryItem(
                x.MaDanhMuc, x.TenDanhMuc, x.MaDanhMucCha, x.HinhAnh, x.ThuTuHienThi,
                x.MoTa, x.TrangThaiHoatDong, x.SanPhams.Count, x.LoaiDanhMuc, x.ApDungKhungGio, x.GioBatDau, x.GioKetThuc,
                x.ToiThieuChon, x.ToiDaChon, x.LaNhomKichThuoc))
            .ToListAsync();
    }

    public async Task<DanhMuc> CreateAsync(CreateCategoryRequest req)
    {
        var name = req.TenDanhMuc.Trim();
        if (await _db.DanhMucs.AnyAsync(x => x.TenDanhMuc.ToLower() == name.ToLower()))
            throw new ArgumentException($"Tên danh mục '{name}' đã tồn tại.");

        if (req.LoaiDanhMuc == "TOPPING")
        {
            if (req.ToiThieuChon < 0) throw new ArgumentException("Tối thiểu chọn phải >= 0.");
            if (req.ToiDaChon <= 0) throw new ArgumentException("Tối đa chọn phải > 0.");
            if (req.ToiThieuChon > req.ToiDaChon) throw new ArgumentException("Tối đa chọn phải >= Tối thiểu chọn.");
        }

        var dm = new DanhMuc
        {
            TenDanhMuc = req.TenDanhMuc.Trim(),
            MaDanhMucCha = req.MaDanhMucCha,
            HinhAnh = req.HinhAnh,
            ThuTuHienThi = req.ThuTuHienThi,
            MoTa = req.MoTa,
            TrangThaiHoatDong = req.TrangThaiHoatDong,
            LoaiDanhMuc = req.LoaiDanhMuc,
            ApDungKhungGio = req.ApDungKhungGio,
            GioBatDau = req.GioBatDau,
            GioKetThuc = req.GioKetThuc,
            ToiThieuChon = req.ToiThieuChon,
            ToiDaChon = req.ToiDaChon,
            LaNhomKichThuoc = req.LaNhomKichThuoc
        };
        _db.DanhMucs.Add(dm);
        await _db.SaveChangesAsync();
        return dm;
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest req)
    {
        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm is null) return false;
        if (req.MaDanhMucCha == id) throw new ArgumentException("Danh mục không thể là cha của chính nó.");

        var name = req.TenDanhMuc.Trim();
        if (await _db.DanhMucs.AnyAsync(x => x.TenDanhMuc.ToLower() == name.ToLower() && x.MaDanhMuc != id))
            throw new ArgumentException($"Tên danh mục '{name}' đã tồn tại.");

        if (req.LoaiDanhMuc == "TOPPING")
        {
            if (req.ToiThieuChon < 0) throw new ArgumentException("Tối thiểu chọn phải >= 0.");
            if (req.ToiDaChon <= 0) throw new ArgumentException("Tối đa chọn phải > 0.");
            if (req.ToiThieuChon > req.ToiDaChon) throw new ArgumentException("Tối đa chọn phải >= Tối thiểu chọn.");
        }

        dm.TenDanhMuc = name;
        dm.MaDanhMucCha = req.MaDanhMucCha;
        dm.HinhAnh = req.HinhAnh;
        dm.ThuTuHienThi = req.ThuTuHienThi;
        dm.MoTa = req.MoTa;
        dm.TrangThaiHoatDong = req.TrangThaiHoatDong;
        dm.LoaiDanhMuc = req.LoaiDanhMuc;
        dm.ApDungKhungGio = req.ApDungKhungGio;
        dm.GioBatDau = req.GioBatDau;
        dm.GioKetThuc = req.GioKetThuc;
        dm.ToiThieuChon = req.ToiThieuChon;
        dm.ToiDaChon = req.ToiDaChon;
        dm.LaNhomKichThuoc = req.LaNhomKichThuoc;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dm = await _db.DanhMucs.FindAsync(id);
        if (dm is null) return false;
        if (await _db.SanPhams.AnyAsync(x => x.MaDanhMuc == id))
            throw new ArgumentException("Danh mục đang có sản phẩm, không thể xoá.");
        if (await _db.DanhMucs.AnyAsync(x => x.MaDanhMucCha == id))
            throw new ArgumentException("Danh mục đang có danh mục con, không thể xoá.");

        _db.DanhMucs.Remove(dm);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task UpdateOrderAsync(List<CategoryOrderRequest> requests)
    {
        var ids = requests.Select(x => x.MaDanhMuc).ToList();
        var dms = await _db.DanhMucs.Where(x => ids.Contains(x.MaDanhMuc)).ToListAsync();

        foreach (var req in requests)
        {
            var dm = dms.FirstOrDefault(x => x.MaDanhMuc == req.MaDanhMuc);
            if (dm != null)
            {
                dm.ThuTuHienThi = req.ThuTuHienThi;
            }
        }
        await _db.SaveChangesAsync();
    }

    public async Task<List<object>> GetCategoryProductsAsync(int categoryId)
    {
        return await _db.SanPhams
            .Where(x => x.MaDanhMuc == categoryId)
            .Select(x => new
            {
                x.MaSanPham,
                x.TenSanPham,
                x.HinhAnh,
                x.GiaBan
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<List<object>> GetAllProductsAsync()
    {
        return await _db.SanPhams
            .Select(x => new
            {
                x.MaSanPham,
                x.TenSanPham,
                x.HinhAnh,
                x.GiaBan,
                x.MaDanhMuc
            })
            .Cast<object>()
            .ToListAsync();
    }

    public async Task<bool> AssignProductsAsync(int categoryId, List<int> productIds)
    {
        var category = await _db.DanhMucs.FindAsync(categoryId);
        if (category == null) return false;

        var currentProducts = await _db.SanPhams.Where(x => x.MaDanhMuc == categoryId).ToListAsync();
        foreach (var p in currentProducts)
        {
            p.MaDanhMuc = null;
        }

        if (productIds.Count > 0)
        {
            var newProducts = await _db.SanPhams.Where(x => productIds.Contains(x.MaSanPham)).ToListAsync();
            foreach (var p in newProducts)
            {
                p.MaDanhMuc = categoryId;
            }
        }

        await _db.SaveChangesAsync();
        return true;
    }
}
