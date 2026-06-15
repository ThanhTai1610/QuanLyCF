using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class InitPhase1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quyen",
                columns: table => new
                {
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenQuyen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Nhom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quyen", x => x.MaQuyen);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LaVaiTroHeThong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "NhanVien",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MatKhauHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaPinHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TokenKhoiPhucMatKhau = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HanTokenKhoiPhuc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoCCCD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SoTaiKhoanNganHang = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenNganHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LuongCoBan = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SoLanDangNhapSai = table.Column<int>(type: "int", nullable: false),
                    KhoaDenKhi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhanVien", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NhanVien_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro_Quyen",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MaQuyen = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro_Quyen", x => new { x.MaVaiTro, x.MaQuyen });
                    table.ForeignKey(
                        name: "FK_VaiTro_Quyen_Quyen_MaQuyen",
                        column: x => x.MaQuyen,
                        principalTable: "Quyen",
                        principalColumn: "MaQuyen",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VaiTro_Quyen_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    MaRefreshToken = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianThuHoi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.MaRefreshToken);
                    table.ForeignKey(
                        name: "FK_RefreshToken_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Quyen",
                columns: new[] { "MaQuyen", "MaCode", "Nhom", "TenQuyen" },
                values: new object[,]
                {
                    { 1, "TAIKHOAN_XEM", "TaiKhoan", "Xem tài khoản" },
                    { 2, "TAIKHOAN_QUANLY", "TaiKhoan", "Quản lý tài khoản" },
                    { 3, "SANPHAM_XEM", "SanPham", "Xem sản phẩm" },
                    { 4, "SANPHAM_QUANLY", "SanPham", "Quản lý sản phẩm" },
                    { 5, "KHO_XEM", "Kho", "Xem kho" },
                    { 6, "KHO_QUANLY", "Kho", "Quản lý kho" },
                    { 7, "DONHANG_XEM", "DonHang", "Xem đơn hàng" },
                    { 8, "DONHANG_XULY", "DonHang", "Xử lý đơn hàng" },
                    { 9, "BEP_XEM", "Bep", "Màn hình bếp" },
                    { 10, "THANHTOAN", "ThanhToan", "Thanh toán" },
                    { 11, "KHACHHANG_XEM", "KhachHang", "Xem khách hàng" },
                    { 12, "KHACHHANG_QUANLY", "KhachHang", "Quản lý khách hàng" },
                    { 13, "NHANSU_XEM", "NhanSu", "Xem nhân sự" },
                    { 14, "NHANSU_QUANLY", "NhanSu", "Quản lý nhân sự" },
                    { 15, "BAOCAO_XEM", "BaoCao", "Xem báo cáo" },
                    { 16, "CAIDAT_QUANLY", "CaiDat", "Quản lý cài đặt" }
                });

            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "MaVaiTro", "LaVaiTroHeThong", "MoTa", "TenVaiTro" },
                values: new object[,]
                {
                    { 1, true, "Toàn quyền hệ thống", "Quản lý" },
                    { 2, true, "Bếp, đơn hàng, xem kho", "Pha chế" },
                    { 3, true, "Bán hàng, thu ngân, hoá đơn", "Thu ngân" },
                    { 4, true, "Đơn hàng, bàn, phục vụ", "Phục vụ" },
                    { 5, true, "Màn hình bếp", "Bếp" }
                });

            migrationBuilder.InsertData(
                table: "VaiTro_Quyen",
                columns: new[] { "MaQuyen", "MaVaiTro" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 1 },
                    { 3, 1 },
                    { 4, 1 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 12, 1 },
                    { 13, 1 },
                    { 14, 1 },
                    { 15, 1 },
                    { 16, 1 },
                    { 3, 2 },
                    { 5, 2 },
                    { 7, 2 },
                    { 8, 2 },
                    { 9, 2 },
                    { 3, 3 },
                    { 7, 3 },
                    { 8, 3 },
                    { 10, 3 },
                    { 11, 3 },
                    { 3, 4 },
                    { 7, 4 },
                    { 8, 4 },
                    { 7, 5 },
                    { 9, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_Email",
                table: "NhanVien",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_MaVaiTro",
                table: "NhanVien",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NhanVien_SoCCCD",
                table: "NhanVien",
                column: "SoCCCD",
                unique: true,
                filter: "[SoCCCD] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Quyen_MaCode",
                table: "Quyen",
                column: "MaCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_MaNhanVien",
                table: "RefreshToken",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_Quyen_MaQuyen",
                table: "VaiTro_Quyen",
                column: "MaQuyen");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "VaiTro_Quyen");

            migrationBuilder.DropTable(
                name: "NhanVien");

            migrationBuilder.DropTable(
                name: "Quyen");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
