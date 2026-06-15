using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddAllModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BangLuong",
                columns: table => new
                {
                    MaBangLuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    Ky = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    LuongTheoGio = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SoGioThuong = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    SoGioOT = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    SoNgayPhep = table.Column<decimal>(type: "decimal(5,1)", nullable: false),
                    PhuCap = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Thuong = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Phat = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    ThucLanh = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BangLuong", x => x.MaBangLuong);
                    table.ForeignKey(
                        name: "FK_BangLuong_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaiDatHeThong",
                columns: table => new
                {
                    MaCaiDat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NhomCaiDat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KhoaCaiDat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GiaTriCaiDat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaiDatHeThong", x => x.MaCaiDat);
                });

            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    MaCa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamViec", x => x.MaCa);
                });

            migrationBuilder.CreateTable(
                name: "DongTien",
                columns: table => new
                {
                    MaDongTien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiGiaoDich = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NhomGiaoDich = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaThamChieu = table.Column<int>(type: "int", nullable: true),
                    PhuongThucThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    NguoiNopNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChungTuDinhKem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MaNhanVienGhiNhan = table.Column<int>(type: "int", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DongTien", x => x.MaDongTien);
                    table.ForeignKey(
                        name: "FK_DongTien_NhanVien_MaNhanVienGhiNhan",
                        column: x => x.MaNhanVienGhiNhan,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonTuNhanVien",
                columns: table => new
                {
                    MaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    LoaiDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianLienQuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNguoiDuyet = table.Column<int>(type: "int", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonTuNhanVien", x => x.MaDon);
                    table.ForeignKey(
                        name: "FK_DonTuNhanVien_NhanVien_MaNguoiDuyet",
                        column: x => x.MaNguoiDuyet,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien");
                    table.ForeignKey(
                        name: "FK_DonTuNhanVien_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    MaKhachHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HangThanhVien = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiemTichLuy = table.Column<int>(type: "int", nullable: false),
                    TongTienDaTieu = table.Column<decimal>(type: "decimal(15,2)", nullable: false),
                    LanGheThamCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GhiChuKhachHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.MaKhachHang);
                });

            migrationBuilder.CreateTable(
                name: "KhuyenMai",
                columns: table => new
                {
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaGiamGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenChuongTrinh = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LoaiGiamGia = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaTriGiam = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GiamToiDa = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DonToiThieu = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SoLuongGioiHan = table.Column<int>(type: "int", nullable: true),
                    SoLuongDaDung = table.Column<int>(type: "int", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuyenMai", x => x.MaKhuyenMai);
                });

            migrationBuilder.CreateTable(
                name: "NhatKyHeThong",
                columns: table => new
                {
                    MaNhatKy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    HanhDong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Module = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DuLieuCu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuLieuMoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiIP = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ThietBi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhatKyHeThong", x => x.MaNhatKy);
                    table.ForeignKey(
                        name: "FK_NhatKyHeThong_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PhanThuong",
                columns: table => new
                {
                    MaPhanThuong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhanThuong = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiemCanDoi = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanThuong", x => x.MaPhanThuong);
                });

            migrationBuilder.CreateTable(
                name: "ChamCong",
                columns: table => new
                {
                    MaChamCong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaCa = table.Column<int>(type: "int", nullable: true),
                    ThoiGianVao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianRa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnhVao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AnhRa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoPhutDiTre = table.Column<int>(type: "int", nullable: false),
                    SoPhutVeSom = table.Column<int>(type: "int", nullable: false),
                    LamThemGioPhut = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamCong", x => x.MaChamCong);
                    table.ForeignKey(
                        name: "FK_ChamCong_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChamCong_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhanCaLamViec",
                columns: table => new
                {
                    MaPhanCa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaCa = table.Column<int>(type: "int", nullable: false),
                    NgayLamViec = table.Column<DateOnly>(type: "date", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanCaLamViec", x => x.MaPhanCa);
                    table.ForeignKey(
                        name: "FK_PhanCaLamViec_CaLamViec_MaCa",
                        column: x => x.MaCa,
                        principalTable: "CaLamViec",
                        principalColumn: "MaCa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhanCaLamViec_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichSuChatbot",
                columns: table => new
                {
                    MaLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    PhienChat = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TinNhanKhach = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhanHoiBot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IntentContext = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuChatbot", x => x.MaLichSu);
                    table.ForeignKey(
                        name: "FK_LichSuChatbot_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DonHang",
                columns: table => new
                {
                    MaDonHang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBan = table.Column<int>(type: "int", nullable: true),
                    MaKhachHang = table.Column<int>(type: "int", nullable: true),
                    MaNhanVien = table.Column<int>(type: "int", nullable: true),
                    MaKhuyenMai = table.Column<int>(type: "int", nullable: true),
                    LoaiDonHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TongTienHang = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TienGiamGia = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PhiDichVu = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ThueVAT = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GhiChuDonHang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiDon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LyDoHuy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonHang", x => x.MaDonHang);
                    table.ForeignKey(
                        name: "FK_DonHang_Ban_MaBan",
                        column: x => x.MaBan,
                        principalTable: "Ban",
                        principalColumn: "MaBan",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DonHang_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DonHang_KhuyenMai_MaKhuyenMai",
                        column: x => x.MaKhuyenMai,
                        principalTable: "KhuyenMai",
                        principalColumn: "MaKhuyenMai",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DonHang_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietDonHang",
                columns: table => new
                {
                    MaChiTiet = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<int>(type: "int", nullable: true),
                    MaKichCo = table.Column<int>(type: "int", nullable: true),
                    MaCombo = table.Column<int>(type: "int", nullable: true),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TienGiamGia = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    GhiChuMon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiBep = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVienPhaChe = table.Column<int>(type: "int", nullable: true),
                    BaoHetNguyenLieu = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianBaoBep = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianLamXong = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietDonHang", x => x.MaChiTiet);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_Combo_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "Combo",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_KichCoSanPham_MaKichCo",
                        column: x => x.MaKichCo,
                        principalTable: "KichCoSanPham",
                        principalColumn: "MaKichCo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_NhanVien_MaNhanVienPhaChe",
                        column: x => x.MaNhanVienPhaChe,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ChiTietDonHang_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DanhGia",
                columns: table => new
                {
                    MaDanhGia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaDonHang = table.Column<int>(type: "int", nullable: true),
                    DiemSo = table.Column<int>(type: "int", nullable: false),
                    BinhLuan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HinhAnhDinhKem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhanHoiTuQuanLy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHienThi = table.Column<bool>(type: "bit", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGia", x => x.MaDanhGia);
                    table.CheckConstraint("CK_DanhGia_DiemSo", "[DiemSo] BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_DanhGia_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DanhGia_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HoaDon",
                columns: table => new
                {
                    MaHoaDon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDonHang = table.Column<int>(type: "int", nullable: false),
                    MaNhanVienThuNgan = table.Column<int>(type: "int", nullable: true),
                    MaSoThueXuatHD = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TongThanhTien = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    SoTienKhachTra = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TienThoiLai = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoaDon", x => x.MaHoaDon);
                    table.ForeignKey(
                        name: "FK_HoaDon_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoaDon_NhanVien_MaNhanVienThuNgan",
                        column: x => x.MaNhanVienThuNgan,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "LichSuDiem",
                columns: table => new
                {
                    MaLichSuDiem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhachHang = table.Column<int>(type: "int", nullable: false),
                    MaDonHang = table.Column<int>(type: "int", nullable: true),
                    LoaiBienDong = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoDiem = table.Column<int>(type: "int", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuDiem", x => x.MaLichSuDiem);
                    table.ForeignKey(
                        name: "FK_LichSuDiem_DonHang_MaDonHang",
                        column: x => x.MaDonHang,
                        principalTable: "DonHang",
                        principalColumn: "MaDonHang",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_LichSuDiem_KhachHang_MaKhachHang",
                        column: x => x.MaKhachHang,
                        principalTable: "KhachHang",
                        principalColumn: "MaKhachHang",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThanhToanChiTiet",
                columns: table => new
                {
                    MaThanhToan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHoaDon = table.Column<int>(type: "int", nullable: false),
                    PhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaGiaoDichCong = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThanhToanChiTiet", x => x.MaThanhToan);
                    table.ForeignKey(
                        name: "FK_ThanhToanChiTiet_HoaDon_MaHoaDon",
                        column: x => x.MaHoaDon,
                        principalTable: "HoaDon",
                        principalColumn: "MaHoaDon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "CaiDatHeThong",
                columns: new[] { "MaCaiDat", "GiaTriCaiDat", "KhoaCaiDat", "MoTa", "NhomCaiDat", "ThoiGianCapNhat" },
                values: new object[,]
                {
                    { 1, "BrewManager Coffee", "TEN_QUAN", "Tên quán", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "", "DIA_CHI", "Địa chỉ quán", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "8", "THUE_VAT_MAC_DINH", "Thuế VAT mặc định (%)", "THANH_TOAN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "0", "PHI_DICH_VU", "Phí dịch vụ mặc định (đ)", "THANH_TOAN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "1", "TY_LE_TICH_DIEM", "Số điểm cho mỗi 1.000đ", "TICH_DIEM", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BangLuong_MaNhanVien",
                table: "BangLuong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_CaiDatHeThong_KhoaCaiDat",
                table: "CaiDatHeThong",
                column: "KhoaCaiDat",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_MaCa",
                table: "ChamCong",
                column: "MaCa");

            migrationBuilder.CreateIndex(
                name: "IX_ChamCong_MaNhanVien",
                table: "ChamCong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaCombo",
                table: "ChiTietDonHang",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaDonHang",
                table: "ChiTietDonHang",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaKichCo",
                table: "ChiTietDonHang",
                column: "MaKichCo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaNhanVienPhaChe",
                table: "ChiTietDonHang",
                column: "MaNhanVienPhaChe");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietDonHang_MaSanPham",
                table: "ChiTietDonHang",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaDonHang",
                table: "DanhGia",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGia_MaKhachHang",
                table: "DanhGia",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DongTien_MaNhanVienGhiNhan",
                table: "DongTien",
                column: "MaNhanVienGhiNhan");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaBan",
                table: "DonHang",
                column: "MaBan");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhachHang",
                table: "DonHang",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaKhuyenMai",
                table: "DonHang",
                column: "MaKhuyenMai");

            migrationBuilder.CreateIndex(
                name: "IX_DonHang_MaNhanVien",
                table: "DonHang",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DonTuNhanVien_MaNguoiDuyet",
                table: "DonTuNhanVien",
                column: "MaNguoiDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_DonTuNhanVien_MaNhanVien",
                table: "DonTuNhanVien",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaDonHang",
                table: "HoaDon",
                column: "MaDonHang",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaNhanVienThuNgan",
                table: "HoaDon",
                column: "MaNhanVienThuNgan");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHang_Email",
                table: "KhachHang",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_KhuyenMai_MaGiamGia",
                table: "KhuyenMai",
                column: "MaGiamGia",
                unique: true,
                filter: "[MaGiamGia] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuChatbot_MaKhachHang",
                table: "LichSuChatbot",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiem_MaDonHang",
                table: "LichSuDiem",
                column: "MaDonHang");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuDiem_MaKhachHang",
                table: "LichSuDiem",
                column: "MaKhachHang");

            migrationBuilder.CreateIndex(
                name: "IX_NhatKyHeThong_MaNhanVien",
                table: "NhatKyHeThong",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCaLamViec_MaCa",
                table: "PhanCaLamViec",
                column: "MaCa");

            migrationBuilder.CreateIndex(
                name: "IX_PhanCaLamViec_MaNhanVien",
                table: "PhanCaLamViec",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_ThanhToanChiTiet_MaHoaDon",
                table: "ThanhToanChiTiet",
                column: "MaHoaDon");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BangLuong");

            migrationBuilder.DropTable(
                name: "CaiDatHeThong");

            migrationBuilder.DropTable(
                name: "ChamCong");

            migrationBuilder.DropTable(
                name: "ChiTietDonHang");

            migrationBuilder.DropTable(
                name: "DanhGia");

            migrationBuilder.DropTable(
                name: "DongTien");

            migrationBuilder.DropTable(
                name: "DonTuNhanVien");

            migrationBuilder.DropTable(
                name: "LichSuChatbot");

            migrationBuilder.DropTable(
                name: "LichSuDiem");

            migrationBuilder.DropTable(
                name: "NhatKyHeThong");

            migrationBuilder.DropTable(
                name: "PhanCaLamViec");

            migrationBuilder.DropTable(
                name: "PhanThuong");

            migrationBuilder.DropTable(
                name: "ThanhToanChiTiet");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "HoaDon");

            migrationBuilder.DropTable(
                name: "DonHang");

            migrationBuilder.DropTable(
                name: "KhachHang");

            migrationBuilder.DropTable(
                name: "KhuyenMai");
        }
    }
}
