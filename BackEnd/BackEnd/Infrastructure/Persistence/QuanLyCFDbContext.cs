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

    // Sales - Đơn hàng / Hoá đơn / Khuyến mãi
    public DbSet<KhuyenMai> KhuyenMais => Set<KhuyenMai>();
    public DbSet<DonHang> DonHangs => Set<DonHang>();
    public DbSet<ChiTietDonHang> ChiTietDonHangs => Set<ChiTietDonHang>();
    public DbSet<HoaDon> HoaDons => Set<HoaDon>();
    public DbSet<ThanhToanChiTiet> ThanhToanChiTiets => Set<ThanhToanChiTiet>();

    // Customers
    public DbSet<KhachHang> KhachHangs => Set<KhachHang>();
    public DbSet<PhanThuong> PhanThuongs => Set<PhanThuong>();
    public DbSet<LichSuDiem> LichSuDiems => Set<LichSuDiem>();
    public DbSet<DanhGia> DanhGias => Set<DanhGia>();
    public DbSet<LichSuChatbot> LichSuChatbots => Set<LichSuChatbot>();

    // HR
    public DbSet<CaLamViec> CaLamViecs => Set<CaLamViec>();
    public DbSet<PhanCaLamViec> PhanCaLamViecs => Set<PhanCaLamViec>();
    public DbSet<ChamCong> ChamCongs => Set<ChamCong>();
    public DbSet<DonTuNhanVien> DonTuNhanViens => Set<DonTuNhanVien>();
    public DbSet<BangLuong> BangLuongs => Set<BangLuong>();

    // Finance
    public DbSet<DongTien> DongTiens => Set<DongTien>();

    // System
    public DbSet<NhatKyHeThong> NhatKyHeThongs => Set<NhatKyHeThong>();
    public DbSet<CaiDatHeThong> CaiDatHeThongs => Set<CaiDatHeThong>();

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
        ConfigOrdering(mb);
        ConfigCustomers(mb);
        ConfigHr(mb);
        ConfigFinanceSystem(mb);

        SeedRbac(mb);
        SeedCatalog(mb);
        SeedSettings(mb);
    }

    private static void ConfigOrdering(ModelBuilder mb)
    {
        mb.Entity<KhuyenMai>(e =>
        {
            e.ToTable("KhuyenMai");
            e.HasKey(x => x.MaKhuyenMai);
            e.Property(x => x.MaGiamGia).HasMaxLength(50);
            e.Property(x => x.TenChuongTrinh).HasMaxLength(150).IsRequired();
            e.Property(x => x.LoaiGiamGia).HasMaxLength(50).IsRequired();
            e.Property(x => x.GiaTriGiam).HasColumnType("decimal(10,2)");
            e.Property(x => x.GiamToiDa).HasColumnType("decimal(10,2)");
            e.Property(x => x.DonToiThieu).HasColumnType("decimal(10,2)");
            e.HasIndex(x => x.MaGiamGia).IsUnique().HasFilter("[MaGiamGia] IS NOT NULL");
        });

        mb.Entity<DonHang>(e =>
        {
            e.ToTable("DonHang");
            e.HasKey(x => x.MaDonHang);
            e.Property(x => x.LoaiDonHang).HasMaxLength(50).IsRequired();
            e.Property(x => x.TrangThaiDon).HasMaxLength(50).IsRequired();
            foreach (var p in new[] { "TongTienHang", "TienGiamGia", "PhiDichVu", "ThueVAT", "ThanhTien" })
                e.Property(p).HasColumnType("decimal(10,2)");
            e.HasOne(x => x.Ban).WithMany().HasForeignKey(x => x.MaBan).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.KhachHang).WithMany().HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.SetNull);
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.KhuyenMai).WithMany().HasForeignKey(x => x.MaKhuyenMai).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<ChiTietDonHang>(e =>
        {
            e.ToTable("ChiTietDonHang");
            e.HasKey(x => x.MaChiTiet);
            e.Property(x => x.TrangThaiBep).HasMaxLength(50).IsRequired();
            e.Property(x => x.DonGia).HasColumnType("decimal(10,2)");
            e.Property(x => x.TienGiamGia).HasColumnType("decimal(10,2)");
            e.Property(x => x.ThanhTien).HasColumnType("decimal(10,2)");
            e.HasOne(x => x.DonHang).WithMany(d => d.ChiTiets).HasForeignKey(x => x.MaDonHang).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.SanPham).WithMany().HasForeignKey(x => x.MaSanPham).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.KichCo).WithMany().HasForeignKey(x => x.MaKichCo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Combo).WithMany().HasForeignKey(x => x.MaCombo).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NhanVienPhaChe).WithMany().HasForeignKey(x => x.MaNhanVienPhaChe).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<HoaDon>(e =>
        {
            e.ToTable("HoaDon");
            e.HasKey(x => x.MaHoaDon);
            e.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            e.Property(x => x.MaSoThueXuatHD).HasMaxLength(50);
            e.Property(x => x.TongThanhTien).HasColumnType("decimal(10,2)");
            e.Property(x => x.SoTienKhachTra).HasColumnType("decimal(10,2)");
            e.Property(x => x.TienThoiLai).HasColumnType("decimal(10,2)");
            e.HasIndex(x => x.MaDonHang).IsUnique();
            e.HasOne(x => x.DonHang).WithOne().HasForeignKey<HoaDon>(x => x.MaDonHang).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NhanVienThuNgan).WithMany().HasForeignKey(x => x.MaNhanVienThuNgan).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<ThanhToanChiTiet>(e =>
        {
            e.ToTable("ThanhToanChiTiet");
            e.HasKey(x => x.MaThanhToan);
            e.Property(x => x.PhuongThuc).HasMaxLength(50).IsRequired();
            e.Property(x => x.SoTien).HasColumnType("decimal(10,2)");
            e.Property(x => x.MaGiaoDichCong).HasMaxLength(100);
            e.HasOne(x => x.HoaDon).WithMany(h => h.ChiTietThanhToans).HasForeignKey(x => x.MaHoaDon).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigCustomers(ModelBuilder mb)
    {
        mb.Entity<KhachHang>(e =>
        {
            e.ToTable("KhachHang");
            e.HasKey(x => x.MaKhachHang);
            e.Property(x => x.Email).HasMaxLength(100);
            e.Property(x => x.HoTen).HasMaxLength(100);
            e.Property(x => x.SoDienThoai).HasMaxLength(20);
            e.Property(x => x.GioiTinh).HasMaxLength(10);
            e.Property(x => x.HangThanhVien).HasMaxLength(50).IsRequired();
            e.Property(x => x.TongTienDaTieu).HasColumnType("decimal(15,2)");
            e.HasIndex(x => x.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
        });

        mb.Entity<PhanThuong>(e =>
        {
            e.ToTable("PhanThuong");
            e.HasKey(x => x.MaPhanThuong);
            e.Property(x => x.TenPhanThuong).HasMaxLength(150).IsRequired();
        });

        mb.Entity<LichSuDiem>(e =>
        {
            e.ToTable("LichSuDiem");
            e.HasKey(x => x.MaLichSuDiem);
            e.Property(x => x.LoaiBienDong).HasMaxLength(20).IsRequired();
            e.HasOne(x => x.KhachHang).WithMany().HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DonHang).WithMany().HasForeignKey(x => x.MaDonHang).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<DanhGia>(e =>
        {
            e.ToTable("DanhGia", t => t.HasCheckConstraint("CK_DanhGia_DiemSo", "[DiemSo] BETWEEN 1 AND 5"));
            e.HasKey(x => x.MaDanhGia);
            e.HasOne(x => x.KhachHang).WithMany().HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.DonHang).WithMany().HasForeignKey(x => x.MaDonHang).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<LichSuChatbot>(e =>
        {
            e.ToTable("LichSuChatbot");
            e.HasKey(x => x.MaLichSu);
            e.Property(x => x.PhienChat).HasMaxLength(100);
            e.Property(x => x.IntentContext).HasMaxLength(100);
            e.HasOne(x => x.KhachHang).WithMany().HasForeignKey(x => x.MaKhachHang).OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigHr(ModelBuilder mb)
    {
        mb.Entity<CaLamViec>(e =>
        {
            e.ToTable("CaLamViec");
            e.HasKey(x => x.MaCa);
            e.Property(x => x.TenCa).HasMaxLength(50).IsRequired();
        });

        mb.Entity<PhanCaLamViec>(e =>
        {
            e.ToTable("PhanCaLamViec");
            e.HasKey(x => x.MaPhanCa);
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Ca).WithMany().HasForeignKey(x => x.MaCa).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<ChamCong>(e =>
        {
            e.ToTable("ChamCong");
            e.HasKey(x => x.MaChamCong);
            e.Property(x => x.TrangThai).HasMaxLength(50);
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Ca).WithMany().HasForeignKey(x => x.MaCa).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<DonTuNhanVien>(e =>
        {
            e.ToTable("DonTuNhanVien");
            e.HasKey(x => x.MaDon);
            e.Property(x => x.LoaiDon).HasMaxLength(50).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Cascade);
            e.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNguoiDuyet).OnDelete(DeleteBehavior.NoAction);
        });

        mb.Entity<BangLuong>(e =>
        {
            e.ToTable("BangLuong");
            e.HasKey(x => x.MaBangLuong);
            e.Property(x => x.Ky).HasMaxLength(7).IsRequired();
            e.Property(x => x.TrangThai).HasMaxLength(50).IsRequired();
            e.Property(x => x.LuongTheoGio).HasColumnType("decimal(12,2)");
            foreach (var p in new[] { "SoGioThuong", "SoGioOT" }) e.Property(p).HasColumnType("decimal(8,2)");
            e.Property(x => x.SoNgayPhep).HasColumnType("decimal(5,1)");
            foreach (var p in new[] { "PhuCap", "Thuong", "Phat", "ThucLanh" }) e.Property(p).HasColumnType("decimal(12,2)");
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigFinanceSystem(ModelBuilder mb)
    {
        mb.Entity<DongTien>(e =>
        {
            e.ToTable("DongTien");
            e.HasKey(x => x.MaDongTien);
            e.Property(x => x.LoaiGiaoDich).HasMaxLength(20).IsRequired();
            e.Property(x => x.NhomGiaoDich).HasMaxLength(100).IsRequired();
            e.Property(x => x.PhuongThucThanhToan).HasMaxLength(50).IsRequired();
            e.Property(x => x.SoTien).HasColumnType("decimal(12,2)");
            e.Property(x => x.ChungTuDinhKem).HasMaxLength(500);
            e.HasOne(x => x.NhanVienGhiNhan).WithMany().HasForeignKey(x => x.MaNhanVienGhiNhan).OnDelete(DeleteBehavior.Restrict);
        });

        mb.Entity<NhatKyHeThong>(e =>
        {
            e.ToTable("NhatKyHeThong");
            e.HasKey(x => x.MaNhatKy);
            e.Property(x => x.HanhDong).HasMaxLength(255).IsRequired();
            e.Property(x => x.Module).HasMaxLength(100).IsRequired();
            e.Property(x => x.DiaChiIP).HasMaxLength(50);
            e.Property(x => x.ThietBi).HasMaxLength(255);
            e.HasOne(x => x.NhanVien).WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.SetNull);
        });

        mb.Entity<CaiDatHeThong>(e =>
        {
            e.ToTable("CaiDatHeThong");
            e.HasKey(x => x.MaCaiDat);
            e.Property(x => x.NhomCaiDat).HasMaxLength(50).IsRequired();
            e.Property(x => x.KhoaCaiDat).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.KhoaCaiDat).IsUnique();
        });
    }

    private static void SeedSettings(ModelBuilder mb)
    {
        mb.Entity<CaiDatHeThong>().HasData(
            new CaiDatHeThong { MaCaiDat = 1, NhomCaiDat = "CHUNG", KhoaCaiDat = "TEN_QUAN", GiaTriCaiDat = "BrewManager Coffee", MoTa = "Tên quán" },
            new CaiDatHeThong { MaCaiDat = 2, NhomCaiDat = "CHUNG", KhoaCaiDat = "DIA_CHI", GiaTriCaiDat = "123 Nguyễn Huệ, Quận 1, TP.HCM", MoTa = "Địa chỉ quán" },
            new CaiDatHeThong { MaCaiDat = 3, NhomCaiDat = "THANH_TOAN", KhoaCaiDat = "THUE_VAT_MAC_DINH", GiaTriCaiDat = "8", MoTa = "Thuế VAT mặc định (%)" },
            new CaiDatHeThong { MaCaiDat = 4, NhomCaiDat = "THANH_TOAN", KhoaCaiDat = "PHI_DICH_VU", GiaTriCaiDat = "0", MoTa = "Phí dịch vụ mặc định (đ)" },
            new CaiDatHeThong { MaCaiDat = 5, NhomCaiDat = "TICH_DIEM", KhoaCaiDat = "TY_LE_TICH_DIEM", GiaTriCaiDat = "1", MoTa = "Số điểm cho mỗi 1.000đ" },
            new CaiDatHeThong { MaCaiDat = 6, NhomCaiDat = "CHUNG", KhoaCaiDat = "CHE_DO_BAO_TRI", GiaTriCaiDat = "false", MoTa = "Chế độ bảo trì hệ thống" },
            new CaiDatHeThong { MaCaiDat = 7, NhomCaiDat = "CHUNG", KhoaCaiDat = "THONG_DIEP_BAO_TRI", GiaTriCaiDat = "Hệ thống đang bảo trì để nâng cấp định kỳ. Vui lòng quay lại sau.", MoTa = "Thông điệp bảo trì" },
            new CaiDatHeThong { MaCaiDat = 8, NhomCaiDat = "CHUNG", KhoaCaiDat = "SO_DIEN_THOAI", GiaTriCaiDat = "0909 123 456", MoTa = "Số điện thoại quán" },
            new CaiDatHeThong { MaCaiDat = 9, NhomCaiDat = "CHUNG", KhoaCaiDat = "MO_TA_QUAN", GiaTriCaiDat = "Quán cà phê đặc sản với không gian ấm cúng. Phục vụ cà phê pha máy, trà, bánh ngọt và các loại đồ uống đá xay.", MoTa = "Mô tả quán" }
        );
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
            // Ghép bàn: tự tham chiếu tới bàn chính
            e.HasOne(x => x.BanChinh).WithMany()
                .HasForeignKey(x => x.MaBanChinh).OnDelete(DeleteBehavior.NoAction);
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
