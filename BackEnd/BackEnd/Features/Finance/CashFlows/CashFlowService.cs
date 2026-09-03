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

        var query = _db.DongTiens.Include(x => x.NhanVienGhiNhan)
            .Where(x => x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month)
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
            x.NhanVienGhiNhan.HoTen
        )).ToListAsync();
    }

    public async Task<CashFlowSummary> TinhTongKetAsync(int year, int month)
    {
        await DamBaoDuLieuKhaoSatAsync(year, month);

        var list = await _db.DongTiens
            .Where(x => x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month)
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

        // One-time fix for existing mock times (moving 21:30 to 06:00 and 14:00 to 07:00)
        var oldRevenueMocks = await _db.DongTiens
            .Where(x => x.NhomGiaoDich == "DoanhThuPOS" && x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month && x.ThoiGianTao.Hour == 21 && x.ThoiGianTao.Minute == 30)
            .ToListAsync();
        if (oldRevenueMocks.Any())
        {
            foreach (var r in oldRevenueMocks) r.ThoiGianTao = new DateTime(r.ThoiGianTao.Year, r.ThoiGianTao.Month, r.ThoiGianTao.Day, 6, 0, 0, DateTimeKind.Utc);
            await _db.SaveChangesAsync();
        }

        var oldImportMocks = await _db.DongTiens
            .Where(x => x.NhomGiaoDich == "NhapHang" && x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month && x.ThoiGianTao.Hour == 14 && x.ThoiGianTao.Minute == 0)
            .ToListAsync();
        if (oldImportMocks.Any())
        {
            foreach (var r in oldImportMocks) r.ThoiGianTao = new DateTime(r.ThoiGianTao.Year, r.ThoiGianTao.Month, r.ThoiGianTao.Day, 7, 0, 0, DateTimeKind.Utc);
            await _db.SaveChangesAsync();
        }

        // 2. Sinh dòng tiền DongTien nếu chưa có giao dịch nào của tháng này
        var existsDongTien = await _db.DongTiens.AnyAsync(x => x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month);
        if (!existsDongTien)
        {
            var random = new Random();
            var nhanVienDuyet = await _db.NhanViens.FirstOrDefaultAsync(x => x.TrangThaiHoatDong == true) ?? new NhanVien { MaNhanVien = 1 };
            var listDongTien = new List<DongTien>();

            // Lấy số ngày của tháng (nếu là tháng hiện tại thì chỉ sinh đến ngày hiện tại)
            var daysInMonth = DateTime.DaysInMonth(year, month);
            if (year == DateTime.UtcNow.Year && month == DateTime.UtcNow.Month)
            {
                daysInMonth = DateTime.UtcNow.Day;
            }

            // A. Chi phí cố định mặt bằng (đầu tháng)
            listDongTien.Add(new DongTien
            {
                LoaiGiaoDich = "Chi",
                NhomGiaoDich = "MatBang",
                PhuongThucThanhToan = "ChuyenKhoan",
                SoTien = 15000000,
                NguoiNopNhan = "Chủ nhà số 123",
                GhiChu = $"Chi phí thuê mặt bằng {ky}",
                MaNhanVienGhiNhan = nhanVienDuyet.MaNhanVien,
                ThoiGianTao = new DateTime(year, month, 1, 9, 0, 0, DateTimeKind.Utc)
            });

            // B. Chi phí điện nước (ngày 5)
            if (daysInMonth >= 5)
            {
                listDongTien.Add(new DongTien
                {
                    LoaiGiaoDich = "Chi",
                    NhomGiaoDich = "DienNuoc",
                    PhuongThucThanhToan = "ChuyenKhoan",
                    SoTien = random.Next(20, 35) * 100000, // 2tr - 3.5tr
                    NguoiNopNhan = "Điện lực & Cấp nước Quận 1",
                    GhiChu = $"Hóa đơn điện nước kinh doanh {ky}",
                    MaNhanVienGhiNhan = nhanVienDuyet.MaNhanVien,
                    ThoiGianTao = new DateTime(year, month, 5, 10, 0, 0, DateTimeKind.Utc)
                });
            }

            // C. Chi phí lương (ngày 10)
            if (daysInMonth >= 10 && tongLuong > 0)
            {
                listDongTien.Add(new DongTien
                {
                    LoaiGiaoDich = "Chi",
                    NhomGiaoDich = "TraLuong",
                    PhuongThucThanhToan = "ChuyenKhoan",
                    SoTien = tongLuong,
                    NguoiNopNhan = "Tập thể nhân viên",
                    GhiChu = $"Thanh toán lương nhân sự kì {ky}",
                    MaNhanVienGhiNhan = nhanVienDuyet.MaNhanVien,
                    ThoiGianTao = new DateTime(year, month, 10, 15, 0, 0, DateTimeKind.Utc)
                });
            }

            // D. Doanh thu POS hằng ngày và chi phí nhập hàng rải rác
            for (int d = 1; d <= daysInMonth; d++)
            {
                // Doanh thu (ngày nào cũng có thu)
                var dailyRevenue = random.Next(25, 60) * 100000; // 2.5tr - 6tr
                listDongTien.Add(new DongTien
                {
                    LoaiGiaoDich = "Thu",
                    NhomGiaoDich = "DoanhThuPOS",
                    PhuongThucThanhToan = "ChuyenKhoan",
                    SoTien = dailyRevenue,
                    NguoiNopNhan = "Khách hàng POS",
                    GhiChu = $"Tổng doanh thu bán hàng ngày {d}/{month:D2}",
                    MaNhanVienGhiNhan = nhanVienDuyet.MaNhanVien,
                    ThoiGianTao = new DateTime(year, month, d, 6, 0, 0, DateTimeKind.Utc)
                });

                // Nhập hàng (2-3 ngày 1 lần)
                if (d % 3 == 1)
                {
                    var importCost = random.Next(5, 15) * 200000; // 1tr - 3tr
                    listDongTien.Add(new DongTien
                    {
                        LoaiGiaoDich = "Chi",
                        NhomGiaoDich = "NhapHang",
                        PhuongThucThanhToan = "TienMat",
                        SoTien = importCost,
                        NguoiNopNhan = "Nhà cung cấp Nguyên liệu",
                        GhiChu = $"Chi phí nhập nguyên liệu định kỳ ngày {d}/{month:D2}",
                        MaNhanVienGhiNhan = nhanVienDuyet.MaNhanVien,
                        ThoiGianTao = new DateTime(year, month, d, 7, 0, 0, DateTimeKind.Utc)
                    });
                }
            }

            if (listDongTien.Any())
            {
                _db.DongTiens.AddRange(listDongTien);
                await _db.SaveChangesAsync();
            }
        }
    }
}
