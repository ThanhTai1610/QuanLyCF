using BackEnd.Domain.Entities;
using BackEnd.Features.Inventory.StockReceipts;
using BackEnd.Infrastructure.Persistence;
using BackEnd.Shared;
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
            x.NhanVienGhiNhan != null ? x.NhanVienGhiNhan.HoTen : "Hệ thống"
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
            .Where(x => x.Ky == ky && (x.NhanVien == null || (x.NhanVien.HoTen != "Quản trị viên" && (x.NhanVien.VaiTro == null || x.NhanVien.VaiTro.TenVaiTro != "Quản trị viên"))));

        return await query.Select(x => new SalaryListItem(
            x.MaBangLuong,
            x.NhanVien != null ? x.NhanVien.HoTen : "Nhân viên",
            (x.NhanVien != null && x.NhanVien.VaiTro != null) ? x.NhanVien.VaiTro.TenVaiTro : "Nhân viên",
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

        // 0. Đảm bảo nhân viên mặc định tồn tại trong CSDL để không vi phạm FK
        var adminNv = await _db.NhanViens.FirstOrDefaultAsync();
        if (adminNv == null)
        {
            adminNv = new NhanVien
            {
                HoTen = "Quản trị viên",
                Email = "admin@brew.vn",
                MaVaiTro = 1,
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("2006"),
                TrangThaiHoatDong = true,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            };
            _db.NhanViens.Add(adminNv);
            await _db.SaveChangesAsync();
        }
        int defaultNvId = adminNv.MaNhanVien;

        // Đảm bảo có ít nhất các nhân viên mẫu khác ngoại trừ Admin
        var nhanViens = await _db.NhanViens.Include(x => x.VaiTro)
            .Where(x => x.TrangThaiHoatDong == true && x.HoTen != "Quản trị viên" && (x.VaiTro == null || x.VaiTro.TenVaiTro != "Quản trị viên"))
            .ToListAsync();

        if (!nhanViens.Any())
        {
            var sampleStaff = new List<NhanVien>
            {
                new NhanVien { HoTen = "Nguyễn Văn Pha", Email = "phache@brew.vn", MaVaiTro = 2, MatKhauHash = PasswordHasher.Hash("demo1234"), MaPinHash = PasswordHasher.Hash("1234"), LuongCoBan = 6000000, TrangThaiHoatDong = true, ThoiGianTao = DateTime.UtcNow, ThoiGianCapNhat = DateTime.UtcNow },
                new NhanVien { HoTen = "Trần Thị Thu", Email = "thungan@brew.vn", MaVaiTro = 3, MatKhauHash = PasswordHasher.Hash("demo1234"), MaPinHash = PasswordHasher.Hash("5678"), LuongCoBan = 5500000, TrangThaiHoatDong = true, ThoiGianTao = DateTime.UtcNow, ThoiGianCapNhat = DateTime.UtcNow },
                new NhanVien { HoTen = "Lê Văn Phục", Email = "phucvu@brew.vn", MaVaiTro = 4, MatKhauHash = PasswordHasher.Hash("demo1234"), MaPinHash = PasswordHasher.Hash("9012"), LuongCoBan = 5000000, TrangThaiHoatDong = true, ThoiGianTao = DateTime.UtcNow, ThoiGianCapNhat = DateTime.UtcNow }
            };
            _db.NhanViens.AddRange(sampleStaff);
            await _db.SaveChangesAsync();
            nhanViens = sampleStaff;
        }

        // 1. Sinh bảng lương trước nếu chưa có để lấy con số chi lương chính xác
        var existsBangLuong = await _db.BangLuongs.AnyAsync(x => x.Ky == ky);
        decimal tongLuong = 0;
        if (!existsBangLuong)
        {
            var listNew = new List<BangLuong>();
            var random = new Random(year * 100 + month);
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

        // 2. Kiểm tra dòng tiền trong tháng này đã có chưa
        var hasDongTienInMonth = await _db.DongTiens.AnyAsync(x => x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month);
        if (!hasDongTienInMonth)
        {
            // Nếu CSDL hoàn toàn rỗng, nạp DongTienSeedData nếu có
            if (!await _db.DongTiens.AnyAsync())
            {
                var seedData = DongTienSeedData.GetSeedData();
                foreach (var s in seedData)
                {
                    s.MaNhanVienGhiNhan = defaultNvId;
                }
                _db.DongTiens.AddRange(seedData);
                await _db.SaveChangesAsync();
                
                // Re-check after seeding
                hasDongTienInMonth = await _db.DongTiens.AnyAsync(x => x.ThoiGianTao.Year == year && x.ThoiGianTao.Month == month);
            }

            if (!hasDongTienInMonth)
            {
                var newDongTiens = new List<DongTien>();
                var rnd = new Random(year * 1000 + month);
                int daysInMonth = DateTime.DaysInMonth(year, month);

                // Chi mặt bằng
                newDongTiens.Add(new DongTien
                {
                    LoaiGiaoDich = "Chi",
                    NhomGiaoDich = "MatBang",
                    PhuongThucThanhToan = "ChuyenKhoan",
                    SoTien = 15000000.00m,
                    NguoiNopNhan = "Chủ nhà số 123",
                    GhiChu = $"Chi phí thuê mặt bằng {ky}",
                    MaNhanVienGhiNhan = defaultNvId,
                    ThoiGianTao = new DateTime(year, month, 1, 9, 0, 0, DateTimeKind.Utc)
                });

                // Chi điện nước
                newDongTiens.Add(new DongTien
                {
                    LoaiGiaoDich = "Chi",
                    NhomGiaoDich = "DienNuoc",
                    PhuongThucThanhToan = "ChuyenKhoan",
                    SoTien = rnd.Next(280, 350) * 10000m,
                    NguoiNopNhan = "Điện lực & Cấp nước Quận 1",
                    GhiChu = $"Hóa đơn điện nước kinh doanh {ky}",
                    MaNhanVienGhiNhan = defaultNvId,
                    ThoiGianTao = new DateTime(year, month, Math.Min(5, daysInMonth), 10, 0, 0, DateTimeKind.Utc)
                });

                // Chi trả lương
                if (tongLuong > 0)
                {
                    newDongTiens.Add(new DongTien
                    {
                        LoaiGiaoDich = "Chi",
                        NhomGiaoDich = "TraLuong",
                        PhuongThucThanhToan = "ChuyenKhoan",
                        SoTien = tongLuong,
                        NguoiNopNhan = "Tập thể nhân viên",
                        GhiChu = $"Thanh toán lương nhân sự kì {ky}",
                        MaNhanVienGhiNhan = defaultNvId,
                        ThoiGianTao = new DateTime(year, month, Math.Min(10, daysInMonth), 15, 0, 0, DateTimeKind.Utc)
                    });
                }

                // Chi nhập hàng và Thu doanh thu POS hàng ngày
                for (int d = 1; d <= daysInMonth; d++)
                {
                    // Nhập hàng định kỳ 3 ngày 1 lần
                    if (d % 3 == 1)
                    {
                        newDongTiens.Add(new DongTien
                        {
                            LoaiGiaoDich = "Chi",
                            NhomGiaoDich = "NhapHang",
                            PhuongThucThanhToan = "TienMat",
                            SoTien = rnd.Next(150, 280) * 10000m,
                            NguoiNopNhan = "Nhà cung cấp Nguyên liệu",
                            GhiChu = $"Chi phí nhập nguyên liệu định kỳ ngày {d}/{month:D2}",
                            MaNhanVienGhiNhan = defaultNvId,
                            ThoiGianTao = new DateTime(year, month, d, 14, 0, 0, DateTimeKind.Utc)
                        });
                    }

                    // Doanh thu POS daily 2-3 giao dịch
                    int txCount = rnd.Next(2, 4);
                    for (int t = 0; t < txCount; t++)
                    {
                        newDongTiens.Add(new DongTien
                        {
                            LoaiGiaoDich = "Thu",
                            NhomGiaoDich = "DoanhThuPOS",
                            PhuongThucThanhToan = "ChuyenKhoan",
                            SoTien = rnd.Next(250, 580) * 10000m,
                            NguoiNopNhan = "Khách hàng POS",
                            GhiChu = $"Tổng doanh thu bán hàng ngày {d}/{month:D2}",
                            MaNhanVienGhiNhan = defaultNvId,
                            ThoiGianTao = new DateTime(year, month, d, 18 + t, 30, 0, DateTimeKind.Utc)
                        });
                    }
                }

                _db.DongTiens.AddRange(newDongTiens);
                await _db.SaveChangesAsync();
            }
        }
    }
}
