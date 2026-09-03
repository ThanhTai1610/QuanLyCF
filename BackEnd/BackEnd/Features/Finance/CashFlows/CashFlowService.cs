using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts;
using BackEnd.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Features.Finance.CashFlows;

public class CashFlowService
{
    private readonly QuanLyCFDbContext _db;
    public CashFlowService(QuanLyCFDbContext db) => _db = db;

    public async Task<List<CashFlowListItem>> LayDanhSachAsync(int year, int month)
    {
        await DamBaoDuLieuKhaoSatAsync(year, month);

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var query = _db.DongTiens.Include(x => x.NhanVienGhiNhan)
            .Where(x => x.ThoiGianTao >= start && x.ThoiGianTao < end)
            .OrderByDescending(x => x.ThoiGianTao);

        return await query.Select(x => new CashFlowListItem(
            x.MaDongTien,
            x.LoaiGiaoDich,
            x.NhomGiaoDich,
            x.SoTien,
            x.PhuongThucThanhToan,
            x.NguoiNopNhan,
            x.GhiChu,
            x.ThoiGianTao,
            x.NhanVienGhiNhan != null ? x.NhanVienGhiNhan.HoTen : "Hệ thống"
        )).ToListAsync();
    }

    public async Task<CashFlowSummary> TinhTongKetAsync(int year, int month)
    {
        await DamBaoDuLieuKhaoSatAsync(year, month);

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        var list = await _db.DongTiens
            .Where(x => x.ThoiGianTao >= start && x.ThoiGianTao < end)
            .GroupBy(x => new { x.LoaiGiaoDich, x.NhomGiaoDich })
            .Select(g => new { g.Key.LoaiGiaoDich, g.Key.NhomGiaoDich, Tong = g.Sum(x => x.SoTien) })
            .ToListAsync();

        var thu = list.Where(x => x.LoaiGiaoDich == "Thu").Sum(x => x.Tong);
        var chi = list.Where(x => x.LoaiGiaoDich == "Chi").Sum(x => x.Tong);

        var chiLuong = list.Where(x => x.NhomGiaoDich == "TraLuong").Sum(x => x.Tong);
        var chiKho = list.Where(x => x.NhomGiaoDich == "NhapHang").Sum(x => x.Tong);
        var chiKhac = chi - chiLuong - chiKho;

        return new CashFlowSummary(thu, chi, thu - chi, chiLuong, chiKho, chiKhac);
    }

    public async Task<List<SalaryListItem>> LayBangLuongAsync(int year, int month)
    {
        await DamBaoDuLieuKhaoSatAsync(year, month);

        var ky = $"{year}-{month:D2}";
        var query = _db.BangLuongs.Include(x => x.NhanVien).ThenInclude(x => x.VaiTro)
            .Where(x => x.Ky == ky && x.NhanVien.HoTen != "Quản trị viên" && (x.NhanVien.VaiTro == null || x.NhanVien.VaiTro.TenVaiTro != "Quản trị viên"));

        return await query.Select(x => new SalaryListItem(
            x.MaBangLuong,
            x.NhanVien.HoTen,
            x.NhanVien.VaiTro.TenVaiTro,
            x.LuongTheoGio,
            x.SoGioThuong,
            x.SoGioOT,
            x.PhuCap,
            x.Thuong,
            x.Phat,
            x.ThucLanh,
            x.TrangThai
        )).ToListAsync();
    }

    public async Task<ServiceResult<int>> TaoPhieuChiAsync(CreateCashOutRequest req, int maNhanVien)
    {
        if (req.SoTien <= 0) return ServiceResult<int>.Fail("Số tiền phải lớn hơn 0.");
        if (string.IsNullOrWhiteSpace(req.NhomGiaoDich)) return ServiceResult<int>.Fail("Vui lòng chọn danh mục chi.");

        var phieu = new DongTien
        {
            LoaiGiaoDich = "Chi",
            NhomGiaoDich = req.NhomGiaoDich,
            PhuongThucThanhToan = req.PhuongThucThanhToan,
            SoTien = req.SoTien,
            NguoiNopNhan = req.NguoiNopNhan,
            GhiChu = req.GhiChu,
            MaNhanVienGhiNhan = maNhanVien,
            ThoiGianTao = DateTime.UtcNow
        };

        _db.DongTiens.Add(phieu);
        await _db.SaveChangesAsync();
        return ServiceResult<int>.Ok(phieu.MaDongTien);
    }

    private async Task DamBaoDuLieuKhaoSatAsync(int year, int month)
    {
        var ky = $"{year}-{month:D2}";
        
        // 1. Sinh bảng lương trước nếu chưa có để lấy con số chi lương chính xác
        var existsBangLuong = await _db.BangLuongs.AnyAsync(x => x.Ky == ky);
        decimal tongLuong = 0;
        if (!existsBangLuong)
        {
            var nhanViens = await _db.NhanViens.Include(x => x.VaiTro)
                .Where(x => x.TrangThaiHoatDong == true && x.HoTen != "Quản trị viên" && (x.VaiTro == null || x.VaiTro.TenVaiTro != "Quản trị viên"))
                .ToListAsync();
            var listNew = new List<BangLuong>();
            var random = new Random();
            foreach (var nv in nhanViens)
            {
                var hourlyRate = (nv.LuongCoBan ?? 0) > 0 ? (nv.LuongCoBan ?? 0) / 200 : 25000;
                if (hourlyRate < 1000) hourlyRate = 25000;
                
                var gioThuong = random.Next(150, 190);
                var gioOt = random.Next(2, 12);
                var phuCap = random.Next(2, 5) * 100000;
                var thuong = random.Next(0, 2) * 200000;
                var phat = random.Next(0, 2) * 50000;
                
                var thucLanh = (hourlyRate * gioThuong) + (hourlyRate * 1.5m * gioOt) + phuCap + thuong - phat;
                tongLuong += thucLanh;

                listNew.Add(new BangLuong
                {
                    MaNhanVien = nv.MaNhanVien,
                    Ky = ky,
                    LuongTheoGio = Math.Round(hourlyRate, 2),
                    SoGioThuong = gioThuong,
                    SoGioOT = gioOt,
                    SoNgayPhep = random.Next(0, 3),
                    PhuCap = phuCap,
                    Thuong = thuong,
                    Phat = phat,
                    ThucLanh = Math.Round(thucLanh, 2),
                    TrangThai = "DaTra",
                    ThoiGianTao = new DateTime(year, month, 10, 8, 0, 0, DateTimeKind.Utc)
                });
            }
            if (listNew.Any())
            {
                _db.BangLuongs.AddRange(listNew);
                await _db.SaveChangesAsync();
            }
        }
        else
        {
            tongLuong = await _db.BangLuongs.Where(x => x.Ky == ky).SumAsync(x => x.ThucLanh);
        }

        var start = new DateTime(year, month, 1);
        var end = start.AddMonths(1);

        // 2. Nạp dữ liệu dòng tiền chính xác từ SQL Server gốc nếu CSDL rỗng
        if (!await _db.DongTiens.AnyAsync())
        {
            _db.DongTiens.AddRange(DongTienSeedData.GetSeedData());
            await _db.SaveChangesAsync();
        }
    }
}
