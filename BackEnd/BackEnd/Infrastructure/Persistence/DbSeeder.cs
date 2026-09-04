using BackEnd.Domain.Entities;
using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence;

public static class DbSeeder
{
    /// <summary>Tạo tài khoản Quản lý mặc định và các dữ liệu mẫu nếu chưa có.</summary>
    public static async Task SeedAsync(QuanLyCFDbContext db)
    {
        // 1. Seed nhân viên quản trị mặc định và các nhân viên mẫu
        if (!await db.NhanViens.AnyAsync(x => x.Email == "admin@brew.vn"))
        {
            db.NhanViens.Add(new NhanVien
            {
                HoTen = "Quản trị viên",
                Email = "admin@brew.vn",
                MaVaiTro = 1, // Quản lý
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("2006"),
                TrangThaiHoatDong = true,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            });
            await db.SaveChangesAsync();
        }

        if (!await db.NhanViens.AnyAsync(x => x.Email == "phache@brew.vn"))
        {
            db.NhanViens.Add(new NhanVien
            {
                HoTen = "Nguyễn Văn Pha",
                Email = "phache@brew.vn",
                MaVaiTro = 2, // Pha chế
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("1234"),
                TrangThaiHoatDong = true,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            });
        }

        if (!await db.NhanViens.AnyAsync(x => x.Email == "thungan@brew.vn"))
        {
            db.NhanViens.Add(new NhanVien
            {
                HoTen = "Trần Thị Thu",
                Email = "thungan@brew.vn",
                MaVaiTro = 3, // Thu ngân
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("5678"),
                TrangThaiHoatDong = true,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            });
        }

        if (!await db.NhanViens.AnyAsync(x => x.Email == "phucvu@brew.vn"))
        {
            db.NhanViens.Add(new NhanVien
            {
                HoTen = "Lê Văn Phục",
                Email = "phucvu@brew.vn",
                MaVaiTro = 4, // Phục vụ
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("9012"),
                TrangThaiHoatDong = true,
                ThoiGianTao = DateTime.UtcNow,
                ThoiGianCapNhat = DateTime.UtcNow
            });
        }
        await db.SaveChangesAsync();

        // 1.5. Đảm bảo phân quyền chuẩn cho các vai trò hệ thống
        var allQuyens = await db.Quyens.ToListAsync();
        int QId(string code) => allQuyens.FirstOrDefault(q => q.MaCode == code)?.MaQuyen ?? 0;

        var defaultRolePermissions = new Dictionary<int, string[]>
        {
            [1] = allQuyens.Select(q => q.MaCode).ToArray(),
            [2] = new[] { "SANPHAM_XEM", "KHO_XEM", "DONHANG_XEM", "DONHANG_XULY", "BEP_XEM", "BAN_XEM", "BAN_QUANLY" },
            [3] = new[] { "SANPHAM_XEM", "DONHANG_XEM", "DONHANG_XULY", "THANHTOAN", "KHACHHANG_XEM", "BAN_XEM", "BAN_QUANLY", "HOADON_XEM" },
            [4] = new[] { "SANPHAM_XEM", "DONHANG_XEM", "DONHANG_XULY", "BAN_XEM", "BAN_QUANLY" },
        };

        foreach (var (vtId, codes) in defaultRolePermissions)
        {
            foreach (var code in codes)
            {
                var qId = QId(code);
                if (qId > 0 && !await db.VaiTroQuyens.AnyAsync(x => x.MaVaiTro == vtId && x.MaQuyen == qId))
                {
                    db.VaiTroQuyens.Add(new VaiTroQuyen { MaVaiTro = vtId, MaQuyen = qId });
                }
            }
        }
        await db.SaveChangesAsync();

        // 2. Seed khu vực và bàn mẫu (nếu chưa có)
        if (!await db.KhuVucBans.AnyAsync())
        {
            var kvTangTret = new KhuVucBan { TenKhuVuc = "Tầng trệt", PhuThu = 0 };
            var kvLau1 = new KhuVucBan { TenKhuVuc = "Lầu 1", PhuThu = 0 };
            var kvSanVuon = new KhuVucBan { TenKhuVuc = "Sân vườn", PhuThu = 0 };

            db.KhuVucBans.AddRange(kvTangTret, kvLau1, kvSanVuon);
            await db.SaveChangesAsync();

            // Thêm bàn cho từng khu vực
            db.Bans.AddRange(
                new Ban { MaKhuVuc = kvTangTret.MaKhuVuc, TenBan = "Bàn 01", SucChua = 4, MaQRHash = "qr-ban-01", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvTangTret.MaKhuVuc, TenBan = "Bàn 02", SucChua = 4, MaQRHash = "qr-ban-02", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvTangTret.MaKhuVuc, TenBan = "Bàn 03", SucChua = 2, MaQRHash = "qr-ban-03", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvTangTret.MaKhuVuc, TenBan = "Bàn 04", SucChua = 6, MaQRHash = "qr-ban-04", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvTangTret.MaKhuVuc, TenBan = "Bàn 05", SucChua = 4, MaQRHash = "qr-ban-05", TrangThai = "Trong" },

                new Ban { MaKhuVuc = kvLau1.MaKhuVuc, TenBan = "Bàn 11", SucChua = 4, MaQRHash = "qr-ban-11", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvLau1.MaKhuVuc, TenBan = "Bàn 12", SucChua = 4, MaQRHash = "qr-ban-12", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvLau1.MaKhuVuc, TenBan = "Bàn 13", SucChua = 2, MaQRHash = "qr-ban-13", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvLau1.MaKhuVuc, TenBan = "Bàn 14", SucChua = 8, MaQRHash = "qr-ban-14", TrangThai = "Trong" },

                new Ban { MaKhuVuc = kvSanVuon.MaKhuVuc, TenBan = "Bàn S1", SucChua = 4, MaQRHash = "qr-ban-s1", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvSanVuon.MaKhuVuc, TenBan = "Bàn S2", SucChua = 4, MaQRHash = "qr-ban-s2", TrangThai = "Trong" },
                new Ban { MaKhuVuc = kvSanVuon.MaKhuVuc, TenBan = "Bàn S3", SucChua = 6, MaQRHash = "qr-ban-s3", TrangThai = "Trong" }
            );
            await db.SaveChangesAsync();
        }

        // 2.0. Seed Danh Mục
        if (!await db.DanhMucs.AnyAsync())
        {
            db.DanhMucs.AddRange(
                new DanhMuc { TenDanhMuc = "Cà phê", ThuTuHienThi = 1 },
                new DanhMuc { TenDanhMuc = "Trà & Trà sữa", ThuTuHienThi = 2 },
                new DanhMuc { TenDanhMuc = "Đá xay", ThuTuHienThi = 3 },
                new DanhMuc { TenDanhMuc = "Bánh ngọt & Ăn nhẹ", ThuTuHienThi = 4 },
                new DanhMuc { TenDanhMuc = "Nước ép & Trái cây", ThuTuHienThi = 5 }
            );
            await db.SaveChangesAsync();
        }

        // 2.1 Seed ca làm việc mẫu (nếu chưa có)
        if (!await db.CaLamViecs.AnyAsync())
        {
            db.CaLamViecs.AddRange(
                new CaLamViec { TenCa = "Ca Sáng", GioBatDau = new TimeOnly(7, 0), GioKetThuc = new TimeOnly(14, 0), TrangThaiHoatDong = true },
                new CaLamViec { TenCa = "Ca Chiều", GioBatDau = new TimeOnly(14, 0), GioKetThuc = new TimeOnly(22, 0), TrangThaiHoatDong = true },
                new CaLamViec { TenCa = "Ca Tối", GioBatDau = new TimeOnly(18, 0), GioKetThuc = new TimeOnly(23, 59), TrangThaiHoatDong = true }
            );
            await db.SaveChangesAsync();
        }

        // 2.2 Seed Lịch phân ca tuần này
        if (!await db.PhanCaLamViecs.AnyAsync())
        {
            var nvPha = await db.NhanViens.FirstOrDefaultAsync(x => x.Email == "phache@brew.vn");
            var nvThu = await db.NhanViens.FirstOrDefaultAsync(x => x.Email == "thungan@brew.vn");
            var nvPhuc = await db.NhanViens.FirstOrDefaultAsync(x => x.Email == "phucvu@brew.vn");
            var caSang = await db.CaLamViecs.FirstOrDefaultAsync(x => x.TenCa == "Ca Sáng");
            var caChieu = await db.CaLamViecs.FirstOrDefaultAsync(x => x.TenCa == "Ca Chiều");

            if (nvPhuc != null && caSang != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                for (int i = -3; i <= 3; i++)
                {
                    db.PhanCaLamViecs.Add(new PhanCaLamViec
                    {
                        MaNhanVien = nvPhuc.MaNhanVien,
                        MaCa = caSang.MaCa,
                        NgayLamViec = today.AddDays(i),
                        GhiChu = "Ca cố định"
                    });
                }
            }

            if (nvPha != null && caChieu != null)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                for (int i = -2; i <= 4; i++)
                {
                    db.PhanCaLamViecs.Add(new PhanCaLamViec
                    {
                        MaNhanVien = nvPha.MaNhanVien,
                        MaCa = caChieu.MaCa,
                        NgayLamViec = today.AddDays(i),
                        GhiChu = "Ca chiều"
                    });
                }
            }

            await db.SaveChangesAsync();
        }

        // 2.3 Seed Khuyến mãi mẫu
        if (!await db.KhuyenMais.AnyAsync())
        {
            db.KhuyenMais.AddRange(
                new KhuyenMai
                {
                    TenChuongTrinh = "Mừng Khai Trương",
                    MaGiamGia = "KHAITRUONG",
                    LoaiGiamGia = "PhanTram",
                    GiaTriGiam = 20,
                    GiamToiDa = 50000,
                    DonToiThieu = 50000,
                    SoLuongGioiHan = 100,
                    SoLuongDaDung = 12,
                    TrangThaiHoatDong = true,
                    NgayBatDau = DateTime.UtcNow.AddDays(-10),
                    NgayKetThuc = DateTime.UtcNow.AddDays(30)
                },
                new KhuyenMai
                {
                    TenChuongTrinh = "Giờ Vàng Cà Phê",
                    MaGiamGia = "COFFEE15",
                    LoaiGiamGia = "PhanTram",
                    GiaTriGiam = 15,
                    GiamToiDa = 30000,
                    DonToiThieu = 30000,
                    SoLuongGioiHan = 200,
                    SoLuongDaDung = 45,
                    TrangThaiHoatDong = true,
                    NgayBatDau = DateTime.UtcNow.AddDays(-5),
                    NgayKetThuc = DateTime.UtcNow.AddDays(15)
                }
            );
            await db.SaveChangesAsync();
        }

        // 3. Seed sản phẩm và toppings (nếu chưa có sản phẩm nào)
        if (!await db.SanPhams.AnyAsync())
        {
            var menu = new List<SanPham>
            {
                // Cà phê (MaDanhMuc = 1)
                new SanPham
                {
                    TenSanPham = "Cà phê đen đá",
                    MaDanhMuc = 1,
                    GiaBan = 20000,
                    GiaVonDuKien = 5000,
                    HinhAnh = "/products/ca-phe-den.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Cà phê sữa đá",
                    MaDanhMuc = 1,
                    GiaBan = 25000,
                    GiaVonDuKien = 7000,
                    HinhAnh = "/products/ca-phe-sua-da.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Bạc xỉu",
                    MaDanhMuc = 1,
                    GiaBan = 29000,
                    GiaVonDuKien = 8000,
                    HinhAnh = "/products/bac-xiu.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },

                // Trà (MaDanhMuc = 2)
                new SanPham
                {
                    TenSanPham = "Trà đào cam sả",
                    MaDanhMuc = 2,
                    GiaBan = 35000,
                    GiaVonDuKien = 12000,
                    HinhAnh = "/products/tra-dao.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Trà vải nhiệt đới",
                    MaDanhMuc = 2,
                    GiaBan = 35000,
                    GiaVonDuKien = 12000,
                    HinhAnh = "/products/tra-vai.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Trà hạt sen thanh mát",
                    MaDanhMuc = 2,
                    GiaBan = 38000,
                    GiaVonDuKien = 13000,
                    HinhAnh = "/products/tra-hat-sen.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Trà sữa truyền thống",
                    MaDanhMuc = 2,
                    GiaBan = 30000,
                    GiaVonDuKien = 10000,
                    HinhAnh = "/products/tra-sua-truyen-thong.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 6000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Trà sữa ô long khói",
                    MaDanhMuc = 2,
                    GiaBan = 39000,
                    GiaVonDuKien = 12000,
                    HinhAnh = "/products/tra-sua-o-long-khoi.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 6000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Hồng trà sữa đặc biệt",
                    MaDanhMuc = 2,
                    GiaBan = 32000,
                    GiaVonDuKien = 11000,
                    HinhAnh = "/products/hong-tra-sua.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 6000 }
                    }
                },

                // Đá xay (MaDanhMuc = 3)
                new SanPham
                {
                    TenSanPham = "Matcha Latte đá xay",
                    MaDanhMuc = 3,
                    GiaBan = 45000,
                    GiaVonDuKien = 15000,
                    HinhAnh = "/products/matcha-latte.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 7000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Sữa tươi trân châu đường đen",
                    MaDanhMuc = 3,
                    GiaBan = 45000,
                    GiaVonDuKien = 15000,
                    HinhAnh = "/products/sua-tuoi-tran-chau-duong-den.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 7000 }
                    }
                },

                // Nước ép / Khác (MaDanhMuc = 5)
                new SanPham
                {
                    TenSanPham = "Nước ép cam tươi",
                    MaDanhMuc = 5,
                    GiaBan = 35000,
                    GiaVonDuKien = 10000,
                    HinhAnh = "/products/ep-cam.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Nước ép cóc",
                    MaDanhMuc = 5,
                    GiaBan = 30000,
                    GiaVonDuKien = 9000,
                    HinhAnh = "/products/ep-coc.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Nước ép táo nguyên chất",
                    MaDanhMuc = 5,
                    GiaBan = 35000,
                    GiaVonDuKien = 10000,
                    HinhAnh = "/products/ep-tao.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },
                new SanPham
                {
                    TenSanPham = "Nước ép thơm (dứa)",
                    MaDanhMuc = 5,
                    GiaBan = 30000,
                    GiaVonDuKien = 9000,
                    HinhAnh = "/products/ep-thom.png",
                    KieuMon = "MonChinh",
                    TrangThaiBan = true,
                    KichCos = new List<KichCoSanPham>
                    {
                        new KichCoSanPham { TenKichCo = "Size M", GiaCongThem = 0 },
                        new KichCoSanPham { TenKichCo = "Size L", GiaCongThem = 5000 }
                    }
                },

                // Toppings (KieuMon = "Topping")
                new SanPham
                {
                    TenSanPham = "Pudding trứng",
                    MaDanhMuc = 5,
                    GiaBan = 8000,
                    GiaVonDuKien = 2000,
                    HinhAnh = "/toppings/pudding.png",
                    KieuMon = "Topping",
                    TrangThaiBan = true
                },
                new SanPham
                {
                    TenSanPham = "Thạch phô mai",
                    MaDanhMuc = 5,
                    GiaBan = 10000,
                    GiaVonDuKien = 3000,
                    HinhAnh = "/toppings/thach_pho_mai.png",
                    KieuMon = "Topping",
                    TrangThaiBan = true
                },
                new SanPham
                {
                    TenSanPham = "Thạch sương sáo",
                    MaDanhMuc = 5,
                    GiaBan = 6000,
                    GiaVonDuKien = 1500,
                    HinhAnh = "/toppings/thach_suong_sao.png",
                    KieuMon = "Topping",
                    TrangThaiBan = true
                },
                new SanPham
                {
                    TenSanPham = "Trân châu đen",
                    MaDanhMuc = 5,
                    GiaBan = 6000,
                    GiaVonDuKien = 1500,
                    HinhAnh = "/toppings/tran_chau_den.png",
                    KieuMon = "Topping",
                    TrangThaiBan = true
                },
                new SanPham
                {
                    TenSanPham = "Trân châu trắng",
                    MaDanhMuc = 5,
                    GiaBan = 8000,
                    GiaVonDuKien = 2000,
                    HinhAnh = "/toppings/tran_chau_trang.png",
                    KieuMon = "Topping",
                    TrangThaiBan = true
                }
            };

            db.SanPhams.AddRange(menu);
            await db.SaveChangesAsync();
        }

        // Cập nhật điểm tích lũy mặc định (1 điểm / ly) cho tất cả món nước trong thực đơn nếu chưa có
        var unpointedProducts = await db.SanPhams.Where(p => p.DiemTichLuy == null || p.DiemTichLuy == 0).ToListAsync();
        if (unpointedProducts.Any())
        {
            foreach (var sp in unpointedProducts)
            {
                if (sp.KieuMon != "Topping")
                {
                    sp.DiemTichLuy = 1; // Mặc định 1 ly = 1 điểm
                }
            }
            await db.SaveChangesAsync();
        }

        // 4. Seed phần thưởng mẫu (nếu chưa có)
        if (!await db.Set<PhanThuong>().AnyAsync())
        {
            db.Set<PhanThuong>().AddRange(
                new PhanThuong { TenPhanThuong = "Free 1 topping", DiemCanDoi = 100, MoTa = "Đổi 100 điểm để nhận miễn phí 1 topping bất kỳ.", TrangThaiHoatDong = true },
                new PhanThuong { TenPhanThuong = "Giảm 10% hóa đơn", DiemCanDoi = 200, MoTa = "Đổi 200 điểm để nhận voucher giảm 10% tổng hóa đơn.", TrangThaiHoatDong = true },
                new PhanThuong { TenPhanThuong = "Tặng 1 ly cà phê", DiemCanDoi = 350, MoTa = "Đổi 350 điểm để nhận miễn phí 1 ly cà phê sữa/đen đá.", TrangThaiHoatDong = true },
                new PhanThuong { TenPhanThuong = "Voucher 50.000đ", DiemCanDoi = 500, MoTa = "Đổi 500 điểm để nhận voucher trị giá 50.000đ.", TrangThaiHoatDong = true }
            );
            await db.SaveChangesAsync();
        }

        // 5. Seed khách hàng mẫu (nếu chưa có)
        if (!await db.Set<KhachHang>().AnyAsync(x => x.SoDienThoai == "0901234567"))
        {
            var kh1 = new KhachHang
            {
                HoTen = "Nguyễn Minh Châu",
                SoDienThoai = "0901234567",
                Email = "chau.nguyen@gmail.com",
                HangThanhVien = "Diamond",
                DiemTichLuy = 4850,
                TongTienDaTieu = 4850000,
                LanGheThamCuoi = DateTime.UtcNow.AddDays(-2),
                ThoiGianTao = DateTime.UtcNow.AddMonths(-3)
            };
            db.Set<KhachHang>().Add(kh1);
            await db.SaveChangesAsync();

            db.Set<LichSuDiem>().AddRange(
                new LichSuDiem { MaKhachHang = kh1.MaKhachHang, LoaiBienDong = "Tich", SoDiem = 100, GhiChu = "Tích điểm mua hàng", ThoiGianTao = DateTime.UtcNow.AddDays(-2) },
                new LichSuDiem { MaKhachHang = kh1.MaKhachHang, LoaiBienDong = "Doi", SoDiem = -200, GhiChu = "Đổi quà Giảm 10% hóa đơn", ThoiGianTao = DateTime.UtcNow.AddDays(-5) }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Set<KhachHang>().AnyAsync(x => x.SoDienThoai == "0912345678"))
        {
            var kh2 = new KhachHang
            {
                HoTen = "Trần Hoàng Linh",
                SoDienThoai = "0912345678",
                Email = "linh.tran@gmail.com",
                HangThanhVien = "Gold",
                DiemTichLuy = 2100,
                TongTienDaTieu = 2100000,
                LanGheThamCuoi = DateTime.UtcNow.AddDays(-5),
                ThoiGianTao = DateTime.UtcNow.AddMonths(-2)
            };
            db.Set<KhachHang>().Add(kh2);
            await db.SaveChangesAsync();

            db.Set<LichSuDiem>().AddRange(
                new LichSuDiem { MaKhachHang = kh2.MaKhachHang, LoaiBienDong = "Tich", SoDiem = 50, GhiChu = "Tích điểm mua hàng", ThoiGianTao = DateTime.UtcNow.AddDays(-5) }
            );
            await db.SaveChangesAsync();
        }

        if (!await db.Set<KhachHang>().AnyAsync(x => x.SoDienThoai == "0923456789"))
        {
            var kh3 = new KhachHang
            {
                HoTen = "Phạm Thị Hương",
                SoDienThoai = "0923456789",
                Email = "huong.pham@gmail.com",
                HangThanhVien = "Silver",
                DiemTichLuy = 980,
                TongTienDaTieu = 980000,
                LanGheThamCuoi = DateTime.UtcNow.AddDays(-10),
                ThoiGianTao = DateTime.UtcNow.AddMonths(-1)
            };
            db.Set<KhachHang>().Add(kh3);
            await db.SaveChangesAsync();
        }

        if (!await db.Set<KhachHang>().AnyAsync(x => x.SoDienThoai == "0934567890"))
        {
            var kh4 = new KhachHang
            {
                HoTen = "Lê Văn Tuấn",
                SoDienThoai = "0934567890",
                Email = "tuan.le@gmail.com",
                HangThanhVien = "Member",
                DiemTichLuy = 320,
                TongTienDaTieu = 320000,
                LanGheThamCuoi = DateTime.UtcNow.AddDays(-15),
                ThoiGianTao = DateTime.UtcNow.AddDays(-20)
            };
            db.Set<KhachHang>().Add(kh4);
            await db.SaveChangesAsync();
        }

        if (!await db.Set<KhachHang>().AnyAsync(x => x.SoDienThoai == "0372700326"))
        {
            var kh5 = new KhachHang
            {
                HoTen = "Nguyễn Văn Thực",
                SoDienThoai = "0372700326",
                Email = "thuc.nguyen@gmail.com",
                HangThanhVien = "Gold",
                DiemTichLuy = 1800,
                TongTienDaTieu = 1800000,
                LanGheThamCuoi = DateTime.UtcNow.AddDays(-2),
                ThoiGianTao = DateTime.UtcNow.AddDays(-30)
            };
            db.Set<KhachHang>().Add(kh5);
            await db.SaveChangesAsync();
        }

        // 6. Seed Bảng Lương từ SQL Server gốc
        if (!await db.BangLuongs.AnyAsync(x => x.Ky == "2026-05"))
        {
            var validNvIds = await db.NhanViens.Select(n => n.MaNhanVien).ToListAsync();
            var defaultNvId = validNvIds.FirstOrDefault();
            var existingKyList = await db.BangLuongs.Select(b => b.Ky).Distinct().ToListAsync();
            var seedSalaries = BangLuongSeedData.GetSeedData();
            var toAdd = seedSalaries.Where(s => !existingKyList.Contains(s.Ky)).ToList();
            if (!await db.BangLuongs.AnyAsync())
            {
                toAdd = seedSalaries;
            }
            if (toAdd.Count > 0)
            {
                foreach (var s in toAdd)
                {
                    if (!validNvIds.Contains(s.MaNhanVien))
                    {
                        s.MaNhanVien = defaultNvId > 0 ? defaultNvId : 1;
                    }
                }
                db.BangLuongs.AddRange(toAdd);
                await db.SaveChangesAsync();
            }
        }

        // 7. Seed Đơn Hàng & Chi Tiết Đơn Hàng từ SQL Server gốc
        if (await db.DonHangs.CountAsync() < 5)
        {
            var validBanIds = await db.Bans.Select(b => b.MaBan).ToListAsync();
            var validKhachIds = await db.KhachHangs.Select(k => k.MaKhachHang).ToListAsync();
            var validPromoIds = await db.KhuyenMais.Select(k => k.MaKhuyenMai).ToListAsync();
            var validSpIds = await db.SanPhams.Select(s => s.MaSanPham).ToListAsync();
            var validKcIds = await db.KichCoSanPhams.Select(kc => kc.MaKichCo).ToListAsync();

            var (rawOrders, rawDetails) = DonHangSeedData.GetSeedData();
            var detailsByOrder = rawDetails.GroupBy(d => d.MaDonHang).ToDictionary(g => g.Key, g => g.ToList());

            var defaultBanId = validBanIds.FirstOrDefault();
            var defaultSpId = validSpIds.FirstOrDefault();

            foreach (var rawOrder in rawOrders)
            {
                var newOrder = new DonHang
                {
                    MaBan = (rawOrder.MaBan.HasValue && validBanIds.Contains(rawOrder.MaBan.Value)) ? rawOrder.MaBan : (defaultBanId > 0 ? defaultBanId : null),
                    LoaiDonHang = rawOrder.LoaiDonHang ?? "DineIn",
                    TrangThaiDon = rawOrder.TrangThaiDon ?? "HoanThanh",
                    TongTienHang = rawOrder.TongTienHang,
                    TienGiamGia = rawOrder.TienGiamGia,
                    ThanhTien = rawOrder.ThanhTien,
                    MaKhachHang = (rawOrder.MaKhachHang.HasValue && validKhachIds.Contains(rawOrder.MaKhachHang.Value)) ? rawOrder.MaKhachHang : null,
                    MaKhuyenMai = (rawOrder.MaKhuyenMai.HasValue && validPromoIds.Contains(rawOrder.MaKhuyenMai.Value)) ? rawOrder.MaKhuyenMai : null,
                    GhiChuDonHang = rawOrder.GhiChuDonHang,
                    ThoiGianTao = rawOrder.ThoiGianTao != default ? rawOrder.ThoiGianTao : DateTime.UtcNow,
                    ThoiGianCapNhat = rawOrder.ThoiGianTao != default ? rawOrder.ThoiGianTao : DateTime.UtcNow
                };

                if (detailsByOrder.TryGetValue(rawOrder.MaDonHang, out var orderDetails))
                {
                    foreach (var rawDet in orderDetails)
                    {
                        var det = new ChiTietDonHang
                        {
                            MaSanPham = (rawDet.MaSanPham.HasValue && validSpIds.Contains(rawDet.MaSanPham.Value)) ? rawDet.MaSanPham : (defaultSpId > 0 ? defaultSpId : null),
                            MaKichCo = (rawDet.MaKichCo.HasValue && validKcIds.Contains(rawDet.MaKichCo.Value)) ? rawDet.MaKichCo : null,
                            SoLuong = rawDet.SoLuong > 0 ? rawDet.SoLuong : 1,
                            DonGia = rawDet.DonGia,
                            TienGiamGia = rawDet.TienGiamGia,
                            ThanhTien = rawDet.ThanhTien,
                            GhiChuMon = rawDet.GhiChuMon,
                            TrangThaiBep = "HoanThanh"
                        };
                        newOrder.ChiTiets.Add(det);
                    }
                }

                db.DonHangs.Add(newOrder);
            }

            await db.SaveChangesAsync();
        }

        // 7.5. Tự động sinh Hóa đơn cho các Đơn hàng đã hoàn tất / đóng bàn mà chưa có Hóa đơn
        var completedOrders = await db.DonHangs
            .Where(d => d.TrangThaiDon == "HoanThanh" || d.TrangThaiDon == "DaDongBan")
            .ToListAsync();

        var existingHoaDonOrderIds = await db.HoaDons.Select(h => h.MaDonHang).ToListAsync();
        var missingHoaDonOrders = completedOrders.Where(o => !existingHoaDonOrderIds.Contains(o.MaDonHang)).ToList();

        if (missingHoaDonOrders.Count > 0)
        {
            foreach (var o in missingHoaDonOrders)
            {
                var hd = new HoaDon
                {
                    MaDonHang = o.MaDonHang,
                    MaNhanVienThuNgan = o.MaNhanVien ?? 1,
                    TongThanhTien = o.ThanhTien,
                    SoTienKhachTra = o.ThanhTien,
                    TienThoiLai = 0,
                    TrangThai = "DaThanhToan",
                    ThoiGianThanhToan = o.ThoiGianCapNhat != default ? o.ThoiGianCapNhat : o.ThoiGianTao
                };
                hd.ChiTietThanhToans.Add(new ThanhToanChiTiet
                {
                    PhuongThuc = "TienMat",
                    SoTien = o.ThanhTien,
                    ThoiGianThanhToan = hd.ThoiGianThanhToan
                });
                db.HoaDons.Add(hd);
            }
            await db.SaveChangesAsync();
        }

        // 8. Seed Dòng Tiền từ SQL Server gốc
        if (!await db.DongTiens.AnyAsync(x => x.GhiChu != null && x.GhiChu.Contains("Chủ nhà số 123")))
        {
            var existingIds = await db.DongTiens.Select(x => x.MaDongTien).ToListAsync();
            var seedCash = DongTienSeedData.GetSeedData();
            var toAdd = seedCash.Where(x => !existingIds.Contains(x.MaDongTien)).ToList();
            if (toAdd.Count > 0)
            {
                db.DongTiens.AddRange(toAdd);
                await db.SaveChangesAsync();
            }
        }
    }
}


