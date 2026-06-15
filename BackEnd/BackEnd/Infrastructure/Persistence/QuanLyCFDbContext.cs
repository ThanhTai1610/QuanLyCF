using BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence;

public class QuanLyCFDbContext : DbContext
{
    public QuanLyCFDbContext(DbContextOptions<QuanLyCFDbContext> options) : base(options) { }

    public DbSet<VaiTro> VaiTros => Set<VaiTro>();
    public DbSet<Quyen> Quyens => Set<Quyen>();
    public DbSet<VaiTroQuyen> VaiTroQuyens => Set<VaiTroQuyen>();
    public DbSet<NhanVien> NhanViens => Set<NhanVien>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    // Catalog
    public DbSet<DanhMuc> DanhMucs => Set<DanhMuc>();
    public DbSet<SanPham> SanPhams => Set<SanPham>();
    public DbSet<KichCoSanPham> KichCoSanPhams => Set<KichCoSanPham>();
    public DbSet<Combo> Combos => Set<Combo>();
    public DbSet<ChiTietCombo> ChiTietCombos => Set<ChiTietCombo>();

    // Inventory
    public DbSet<NguyenLieu> NguyenLieus => Set<NguyenLieu>();
    public DbSet<NhaCungCap> NhaCungCaps => Set<NhaCungCap>();
    public DbSet<PhieuKho> PhieuKhos => Set<PhieuKho>();
    public DbSet<ChiTietPhieuKho> ChiTietPhieuKhos => Set<ChiTietPhieuKho>();

    // Sales - Bàn
    public DbSet<KhuVucBan> KhuVucBans => Set<KhuVucBan>();
    public DbSet<Ban> Bans => Set<Ban>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        // ── VaiTro ──────────────────────────────────────────────
        mb.Entity<VaiTro>(e =>
        {
            e.ToTable("VaiTro");
            e.HasKey(x => x.MaVaiTro);
            e.Property(x => x.TenVaiTro).HasMaxLength(50).IsRequired();
            e.Property(x => x.MoTa).HasMaxLength(500);
            e.HasIndex(x => x.TenVaiTro).IsUnique();
        });

        // ── Quyen ───────────────────────────────────────────────
        mb.Entity<Quyen>(e =>
        {
            e.ToTable("Quyen");
            e.HasKey(x => x.MaQuyen);
            e.Property(x => x.MaCode).HasMaxLength(50).IsRequired();
            e.Property(x => x.TenQuyen).HasMaxLength(150).IsRequired();
            e.Property(x => x.Nhom).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.MaCode).IsUnique();
        });

        // ── VaiTroQuyen (n-n) ───────────────────────────────────
        mb.Entity<VaiTroQuyen>(e =>
        {
            e.ToTable("VaiTro_Quyen");
            e.HasKey(x => new { x.MaVaiTro, x.MaQuyen });
            e.HasOne(x => x.VaiTro).WithMany(v => v.VaiTroQuyens).HasForeignKey(x => x.MaVaiTro).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Quyen).WithMany(q => q.VaiTroQuyens).HasForeignKey(x => x.MaQuyen).OnDelete(DeleteBehavior.Cascade);
        });

        // ── NhanVien ────────────────────────────────────────────
        mb.Entity<NhanVien>(e =>
        {
            e.ToTable("NhanVien");
            e.HasKey(x => x.MaNhanVien);
            e.Property(x => x.HoTen).HasMaxLength(100).IsRequired();
            e.Property(x => x.Email).HasMaxLength(100).IsRequired();
            e.Property(x => x.MatKhauHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.MaPinHash).HasMaxLength(255);
            e.Property(x => x.SoDienThoai).HasMaxLength(20);
            e.Property(x => x.DiaChi).HasMaxLength(255);
            e.Property(x => x.SoCCCD).HasMaxLength(20);
            e.Property(x => x.SoTaiKhoanNganHang).HasMaxLength(50);
            e.Property(x => x.TenNganHang).HasMaxLength(100);
            e.Property(x => x.AnhDaiDien).HasMaxLength(255);
            e.Property(x => x.LuongCoBan).HasColumnType("decimal(12,2)");
            e.Property(x => x.TrangThaiHoatDong).HasDefaultValue(true);
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.SoCCCD).IsUnique().HasFilter("[SoCCCD] IS NOT NULL");
            e.HasOne(x => x.VaiTro).WithMany(v => v.NhanViens).HasForeignKey(x => x.MaVaiTro).OnDelete(DeleteBehavior.Restrict);
        });

        // ── RefreshToken ────────────────────────────────────────
        mb.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshToken");
            e.HasKey(x => x.MaRefreshToken);
            e.Property(x => x.Token).HasMaxLength(255).IsRequired();
            e.HasIndex(x => x.Token);
            e.HasOne(x => x.NhanVien).WithMany(n => n.RefreshTokens).HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Cascade);
            e.Ignore(x => x.ConHieuLuc);
        });

        ConfigCatalog(mb);
        ConfigInventory(mb);
        ConfigSales(mb);

        SeedRbac(mb);
        SeedCatalog(mb);
    }

    private static void ConfigSales(ModelBuilder mb)
    {
        mb.Entity<KhuVucBan>(e =>
        {
            e.ToTable("KhuVucBan");
            e.HasKey(x => x.MaKhuVuc);
            e.Property(x => x.TenKhuVuc).HasMaxLength(100).IsRequired();
            e.Property(x => x.PhuThu).HasColumnType("decimal(10,2)");
        });

        mb.Entity<Ban>(e =>
        {
            e.ToTable("Ban");
            e.HasKey(x => x.MaBan);
            e.Property(x => x.TenBan).HasMaxLength(20).IsRequired();
            e.Property(x => x.MaQRHash).HasMaxLength(255).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            e.HasIndex(x => x.TenBan).IsUnique();
            e.HasIndex(x => x.MaQRHash).IsUnique();
            e.HasOne(x => x.KhuVuc).WithMany(k => k.Bans)
                .HasForeignKey(x => x.MaKhuVuc).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigInventory(ModelBuilder mb)
    {
        mb.Entity<NguyenLieu>(e =>
        {
            e.ToTable("NguyenLieu");
            e.HasKey(x => x.MaNguyenLieu);
            e.Property(x => x.TenNguyenLieu).HasMaxLength(150).IsRequired();
            e.Property(x => x.MaVach_SKU).HasMaxLength(50);
            e.Property(x => x.DonViTinh).HasMaxLength(20).IsRequired();
            e.Property(x => x.SoLuongTon).HasColumnType("decimal(10,3)");
            e.Property(x => x.MucTonToiThieu).HasColumnType("decimal(10,3)");
            e.Property(x => x.MucTonToiDa).HasColumnType("decimal(10,3)");
            e.Property(x => x.GiaVonTrungBinh).HasColumnType("decimal(10,2)");
            e.Property(x => x.HinhAnh).HasMaxLength(255);
            e.HasIndex(x => x.MaVach_SKU).IsUnique().HasFilter("[MaVach_SKU] IS NOT NULL");
        });

        mb.Entity<NhaCungCap>(e =>
        {
            e.ToTable("NhaCungCap");
            e.HasKey(x => x.MaNhaCungCap);
            e.Property(x => x.TenNhaCungCap).HasMaxLength(150).IsRequired();
            e.Property(x => x.MaSoThue).HasMaxLength(50);
            e.Property(x => x.NguoiLienHe).HasMaxLength(100);
            e.Property(x => x.SoDienThoai).HasMaxLength(20);
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.SoTaiKhoan).HasMaxLength(50);
            e.Property(x => x.TenNganHang).HasMaxLength(100);
            e.Property(x => x.CongNoHienTai).HasColumnType("decimal(12,2)");
        });

        mb.Entity<PhieuKho>(e =>
        {
            e.ToTable("PhieuKho");
            e.HasKey(x => x.MaPhieu);
            e.Property(x => x.LoaiPhieu).HasMaxLength(50).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            e.Property(x => x.TrangThaiThanhToan).HasMaxLength(50);
            e.Property(x => x.TongTienHang).HasColumnType("decimal(12,2)");
            e.Property(x => x.TienDaThanhToan).HasColumnType("decimal(12,2)");
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NhaCungCap).WithMany(n => n.PhieuKhos).HasForeignKey(x => x.MaNhaCungCap).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<ChiTietPhieuKho>(e =>
        {
            e.ToTable("ChiTietPhieuKho");
            e.HasKey(x => x.MaChiTietPhieu);
            e.Property(x => x.SoLuong).HasColumnType("decimal(10,3)");
            e.Property(x => x.SoLuongThucTe).HasColumnType("decimal(10,3)");
            e.Property(x => x.DonGia).HasColumnType("decimal(10,2)");
            e.HasOne(x => x.PhieuKho).WithMany(p => p.ChiTiets).HasForeignKey(x => x.MaPhieu).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.NguyenLieu).WithMany(n => n.ChiTietPhieuKhos).HasForeignKey(x => x.MaNguyenLieu).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void ConfigCatalog(ModelBuilder mb)
    {
        mb.Entity<DanhMuc>(e =>
        {
            e.ToTable("DanhMuc");
            e.HasKey(x => x.MaDanhMuc);
            e.Property(x => x.TenDanhMuc).HasMaxLength(100).IsRequired();
            e.Property(x => x.HinhAnh).HasMaxLength(255);
            e.HasOne(x => x.DanhMucCha).WithMany(x => x.DanhMucCon)
                .HasForeignKey(x => x.MaDanhMucCha).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<SanPham>(e =>
        {
            e.ToTable("SanPham");
            e.HasKey(x => x.MaSanPham);
            e.Property(x => x.TenSanPham).HasMaxLength(150).IsRequired();
            e.Property(x => x.MaVach_SKU).HasMaxLength(50);
            e.Property(x => x.HinhAnh).HasMaxLength(255);
            e.Property(x => x.KieuMon).HasMaxLength(50);
            e.Property(x => x.GiaVonDuKien).HasColumnType("decimal(10,2)");
            e.Property(x => x.GiaBan).HasColumnType("decimal(10,2)");
            e.HasIndex(x => x.MaVach_SKU).IsUnique().HasFilter("[MaVach_SKU] IS NOT NULL");
            e.HasOne(x => x.DanhMuc).WithMany(x => x.SanPhams)
                .HasForeignKey(x => x.MaDanhMuc).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<KichCoSanPham>(e =>
        {
            e.ToTable("KichCoSanPham");
            e.HasKey(x => x.MaKichCo);
            e.Property(x => x.TenKichCo).HasMaxLength(50).IsRequired();
            e.Property(x => x.GiaCongThem).HasColumnType("decimal(10,2)");
            e.HasOne(x => x.SanPham).WithMany(x => x.KichCos)
                .HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Cascade);
        });

        mb.Entity<Combo>(e =>
        {
            e.ToTable("Combo");
            e.HasKey(x => x.MaCombo);
            e.Property(x => x.TenCombo).HasMaxLength(150).IsRequired();
            e.Property(x => x.HinhAnh).HasMaxLength(255);
            e.Property(x => x.GiaCombo).HasColumnType("decimal(10,2)");
        });

        mb.Entity<ChiTietCombo>(e =>
        {
            e.ToTable("ChiTietCombo");
            e.HasKey(x => x.MaChiTietCombo);
            e.HasOne(x => x.Combo).WithMany(x => x.ChiTiets)
                .HasForeignKey(x => x.MaCombo).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.SanPham).WithMany()
                .HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Restrict);
        });
    }

    private static void SeedCatalog(ModelBuilder mb)
    {
        mb.Entity<DanhMuc>().HasData(
            new DanhMuc { MaDanhMuc = 1, TenDanhMuc = "Cà phê",  ThuTuHienThi = 1 },
            new DanhMuc { MaDanhMuc = 2, TenDanhMuc = "Trà",     ThuTuHienThi = 2 },
            new DanhMuc { MaDanhMuc = 3, TenDanhMuc = "Đá xay",  ThuTuHienThi = 3 },
            new DanhMuc { MaDanhMuc = 4, TenDanhMuc = "Bánh",    ThuTuHienThi = 4 },
            new DanhMuc { MaDanhMuc = 5, TenDanhMuc = "Khác",    ThuTuHienThi = 5 }
        );
    }

    /// <summary>Seed sẵn vai trò, quyền và gán quyền cho từng vai trò (dữ liệu tĩnh → HasData).</summary>
    private static void SeedRbac(ModelBuilder mb)
    {
        // Vai trò
        var vaiTros = new[]
        {
            new VaiTro { MaVaiTro = 1, TenVaiTro = "Quản lý",  MoTa = "Toàn quyền hệ thống", LaVaiTroHeThong = true },
            new VaiTro { MaVaiTro = 2, TenVaiTro = "Pha chế",  MoTa = "Bếp, đơn hàng, xem kho", LaVaiTroHeThong = true },
            new VaiTro { MaVaiTro = 3, TenVaiTro = "Thu ngân", MoTa = "Bán hàng, thu ngân, hoá đơn", LaVaiTroHeThong = true },
            new VaiTro { MaVaiTro = 4, TenVaiTro = "Phục vụ",  MoTa = "Đơn hàng, bàn, phục vụ", LaVaiTroHeThong = true },
            new VaiTro { MaVaiTro = 5, TenVaiTro = "Bếp",      MoTa = "Màn hình bếp", LaVaiTroHeThong = true },
        };
        mb.Entity<VaiTro>().HasData(vaiTros);

        // Quyền — id cố định theo thứ tự trong Shared.Quyens.TatCa
        var quyenMeta = new (string Code, string Ten, string Nhom)[]
        {
            ("TAIKHOAN_XEM", "Xem tài khoản", "TaiKhoan"),
            ("TAIKHOAN_QUANLY", "Quản lý tài khoản", "TaiKhoan"),
            ("SANPHAM_XEM", "Xem sản phẩm", "SanPham"),
            ("SANPHAM_QUANLY", "Quản lý sản phẩm", "SanPham"),
            ("KHO_XEM", "Xem kho", "Kho"),
            ("KHO_QUANLY", "Quản lý kho", "Kho"),
            ("DONHANG_XEM", "Xem đơn hàng", "DonHang"),
            ("DONHANG_XULY", "Xử lý đơn hàng", "DonHang"),
            ("BEP_XEM", "Màn hình bếp", "Bep"),
            ("THANHTOAN", "Thanh toán", "ThanhToan"),
            ("KHACHHANG_XEM", "Xem khách hàng", "KhachHang"),
            ("KHACHHANG_QUANLY", "Quản lý khách hàng", "KhachHang"),
            ("NHANSU_XEM", "Xem nhân sự", "NhanSu"),
            ("NHANSU_QUANLY", "Quản lý nhân sự", "NhanSu"),
            ("BAOCAO_XEM", "Xem báo cáo", "BaoCao"),
            ("CAIDAT_QUANLY", "Quản lý cài đặt", "CaiDat"),
            ("BAN_XEM", "Xem bàn", "Ban"),
            ("BAN_QUANLY", "Quản lý bàn", "Ban"),
        };
        var quyens = quyenMeta.Select((q, i) => new Quyen { MaQuyen = i + 1, MaCode = q.Code, TenQuyen = q.Ten, Nhom = q.Nhom }).ToArray();
        mb.Entity<Quyen>().HasData(quyens);

        int Id(string code) => Array.FindIndex(quyenMeta, q => q.Code == code) + 1;

        // Gán quyền theo vai trò
        var map = new Dictionary<int, string[]>
        {
            [1] = quyenMeta.Select(q => q.Code).ToArray(), // Quản lý: full
            [2] = new[] { "SANPHAM_XEM", "KHO_XEM", "DONHANG_XEM", "DONHANG_XULY", "BEP_XEM" },                        // Pha chế
            [3] = new[] { "SANPHAM_XEM", "DONHANG_XEM", "DONHANG_XULY", "THANHTOAN", "KHACHHANG_XEM", "BAN_XEM" },     // Thu ngân
            [4] = new[] { "SANPHAM_XEM", "DONHANG_XEM", "DONHANG_XULY", "BAN_XEM" },                                   // Phục vụ
            [5] = new[] { "BEP_XEM", "DONHANG_XEM" },                                                                  // Bếp
        };

        var vtq = new List<VaiTroQuyen>();
        foreach (var (maVaiTro, codes) in map)
            foreach (var code in codes)
                vtq.Add(new VaiTroQuyen { MaVaiTro = maVaiTro, MaQuyen = Id(code) });
        mb.Entity<VaiTroQuyen>().HasData(vtq);
    }

    public override int SaveChanges()
    {
        CapNhatThoiGian();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        CapNhatThoiGian();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Tự gán ThoiGianTao/ThoiGianCapNhat cho NhanVien.</summary>
    private void CapNhatThoiGian()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<NhanVien>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.ThoiGianTao = now;
                entry.Entity.ThoiGianCapNhat = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ThoiGianCapNhat = now;
            }
        }
    }
}
