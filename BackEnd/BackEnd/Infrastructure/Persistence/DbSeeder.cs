using BackEnd.Domain.Entities;
using BackEnd.Shared;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence;

public static class DbSeeder
{
    /// <summary>Tạo tài khoản Quản lý mặc định và các dữ liệu mẫu nếu chưa có.</summary>
    public static async Task SeedAsync(QuanLyCFDbContext db)
    {
        // 1. Seed nhân viên quản trị mặc định
        if (!await db.NhanViens.AnyAsync())
        {
            db.NhanViens.Add(new NhanVien
            {
                HoTen = "Quản trị viên",
                Email = "admin@brew.vn",
                MaVaiTro = 1, // Quản lý
                MatKhauHash = PasswordHasher.Hash("demo1234"),
                MaPinHash = PasswordHasher.Hash("2006"),
                TrangThaiHoatDong = true,
            });
            await db.SaveChangesAsync();
        }

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

        // 4. Seed random orders and invoices for the last 7 days (including today)
        if (!await db.DonHangs.AnyAsync())
        {
            var r = new Random();
            var sanPhams = await db.SanPhams.Include(x => x.KichCos).ToListAsync();
            var bans = await db.Bans.ToListAsync();
            var nvThuNgan = await db.NhanViens.FirstOrDefaultAsync();
            var maNV = nvThuNgan?.MaNhanVien ?? 1;

            if (sanPhams.Any() && bans.Any())
            {
                var donHangs = new List<DonHang>();
                var hoaDons = new List<HoaDon>();
                var today = DateTime.UtcNow.Date;

                // Tạo 7 ngày, mỗi ngày 10-30 đơn
                for (int i = 6; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    int ordersCount = r.Next(10, 30);
                    
                    for (int j = 0; j < ordersCount; j++)
                    {
                        var time = date.AddHours(r.Next(7, 22)).AddMinutes(r.Next(0, 59));
                        
                        var dh = new DonHang
                        {
                            ThoiGianTao = time,
                            ThoiGianCapNhat = time,
                            TrangThaiDon = "HoanThanh",
                            LoaiDonHang = r.Next(10) > 2 ? "DineIn" : "TakeAway",
                            MaBan = bans[r.Next(bans.Count)].MaBan,
                            MaNhanVien = maNV,
                            ChiTiets = new List<ChiTietDonHang>()
                        };

                        if (dh.LoaiDonHang == "TakeAway") dh.MaBan = null;

                        int itemsCount = r.Next(1, 5);
                        decimal tongTien = 0;
                        for (int k = 0; k < itemsCount; k++)
                        {
                            var sp = sanPhams[r.Next(sanPhams.Count)];
                            if (sp.KieuMon == "Topping") { k--; continue; } // ignore topping as main item

                            var size = sp.KichCos?.FirstOrDefault();
                            decimal donGia = sp.GiaBan + (size?.GiaCongThem ?? 0);
                            int soLuong = r.Next(1, 3);
                            decimal thanhTien = donGia * soLuong;
                            tongTien += thanhTien;

                            dh.ChiTiets.Add(new ChiTietDonHang
                            {
                                MaSanPham = sp.MaSanPham,
                                MaKichCo = size?.MaKichCo,
                                SoLuong = soLuong,
                                DonGia = donGia,
                                ThanhTien = thanhTien,
                                TrangThaiBep = "HoanThanh",
                                ThoiGianBaoBep = time,
                                ThoiGianLamXong = time.AddMinutes(r.Next(2, 10))
                            });
                        }

                        if (dh.ChiTiets.Count > 0)
                        {
                            dh.TongTienHang = tongTien;
                            dh.ThanhTien = tongTien;
                            donHangs.Add(dh);
                        }
                    }
                }

                db.DonHangs.AddRange(donHangs);
                await db.SaveChangesAsync(); // save to generate IDs

                foreach (var dh in donHangs)
                {
                    var payTime = dh.ThoiGianCapNhat.AddMinutes(r.Next(5, 30));
                    hoaDons.Add(new HoaDon
                    {
                        MaDonHang = dh.MaDonHang,
                        MaNhanVienThuNgan = dh.MaNhanVien,
                        TongThanhTien = dh.ThanhTien,
                        SoTienKhachTra = dh.ThanhTien,
                        TienThoiLai = 0,
                        TrangThai = "DaThanhToan",
                        ThoiGianThanhToan = payTime,
                        ChiTietThanhToans = new List<ThanhToanChiTiet>
                        {
                            new ThanhToanChiTiet
                            {
                                PhuongThuc = r.Next(10) > 3 ? "TienMat" : "ChuyenKhoan",
                                SoTien = dh.ThanhTien,
                                ThoiGianThanhToan = payTime
                            }
                        }
                    });
                }
                
                db.HoaDons.AddRange(hoaDons);
                await db.SaveChangesAsync();
            }
        }
    }
}
