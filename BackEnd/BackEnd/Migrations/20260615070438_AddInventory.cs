using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NguyenLieu",
                columns: table => new
                {
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguyenLieu = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaVach_SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DonViTinh = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoLuongTon = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    MucTonToiThieu = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    MucTonToiDa = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    GiaVonTrungBinh = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    HanSuDungNgay = table.Column<int>(type: "int", nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguyenLieu", x => x.MaNguyenLieu);
                });

            migrationBuilder.CreateTable(
                name: "NhaCungCap",
                columns: table => new
                {
                    MaNhaCungCap = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhaCungCap = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaSoThue = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NguoiLienHe = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTaiKhoan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TenNganHang = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CongNoHienTai = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhaCungCap", x => x.MaNhaCungCap);
                });

            migrationBuilder.CreateTable(
                name: "PhieuKho",
                columns: table => new
                {
                    MaPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiPhieu = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaNhaCungCap = table.Column<int>(type: "int", nullable: true),
                    TongTienHang = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TienDaThanhToan = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    TrangThaiThanhToan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhieuKho", x => x.MaPhieu);
                    table.ForeignKey(
                        name: "FK_PhieuKho_NhaCungCap_MaNhaCungCap",
                        column: x => x.MaNhaCungCap,
                        principalTable: "NhaCungCap",
                        principalColumn: "MaNhaCungCap",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PhieuKho_NhanVien_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NhanVien",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietPhieuKho",
                columns: table => new
                {
                    MaChiTietPhieu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhieu = table.Column<int>(type: "int", nullable: false),
                    MaNguyenLieu = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<decimal>(type: "decimal(10,3)", nullable: false),
                    DonGia = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    SoLuongThucTe = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    LyDoLech = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietPhieuKho", x => x.MaChiTietPhieu);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuKho_NguyenLieu_MaNguyenLieu",
                        column: x => x.MaNguyenLieu,
                        principalTable: "NguyenLieu",
                        principalColumn: "MaNguyenLieu",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChiTietPhieuKho_PhieuKho_MaPhieu",
                        column: x => x.MaPhieu,
                        principalTable: "PhieuKho",
                        principalColumn: "MaPhieu",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuKho_MaNguyenLieu",
                table: "ChiTietPhieuKho",
                column: "MaNguyenLieu");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietPhieuKho_MaPhieu",
                table: "ChiTietPhieuKho",
                column: "MaPhieu");

            migrationBuilder.CreateIndex(
                name: "IX_NguyenLieu_MaVach_SKU",
                table: "NguyenLieu",
                column: "MaVach_SKU",
                unique: true,
                filter: "[MaVach_SKU] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuKho_MaNhaCungCap",
                table: "PhieuKho",
                column: "MaNhaCungCap");

            migrationBuilder.CreateIndex(
                name: "IX_PhieuKho_MaNhanVien",
                table: "PhieuKho",
                column: "MaNhanVien");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietPhieuKho");

            migrationBuilder.DropTable(
                name: "NguyenLieu");

            migrationBuilder.DropTable(
                name: "PhieuKho");

            migrationBuilder.DropTable(
                name: "NhaCungCap");
        }
    }
}
