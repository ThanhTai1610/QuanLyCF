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
    }
}


