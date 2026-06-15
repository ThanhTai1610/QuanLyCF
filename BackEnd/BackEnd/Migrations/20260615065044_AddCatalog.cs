using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Combo",
                columns: table => new
                {
                    MaCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCombo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    GiaCombo = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Combo", x => x.MaCombo);
                });

            migrationBuilder.CreateTable(
                name: "DanhMuc",
                columns: table => new
                {
                    MaDanhMuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhMucCha = table.Column<int>(type: "int", nullable: true),
                    TenDanhMuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThuTuHienThi = table.Column<int>(type: "int", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhMuc", x => x.MaDanhMuc);
                    table.ForeignKey(
                        name: "FK_DanhMuc_DanhMuc_MaDanhMucCha",
                        column: x => x.MaDanhMucCha,
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SanPham",
                columns: table => new
                {
                    MaSanPham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhMuc = table.Column<int>(type: "int", nullable: true),
                    TenSanPham = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaVach_SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GiaVonDuKien = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    GiaBan = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LuongCalo = table.Column<int>(type: "int", nullable: true),
                    ThoiGianChuanBiPhut = table.Column<int>(type: "int", nullable: true),
                    LaMonNoiBat = table.Column<bool>(type: "bit", nullable: false),
                    KieuMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TrangThaiBan = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanPham", x => x.MaSanPham);
                    table.ForeignKey(
                        name: "FK_SanPham_DanhMuc_MaDanhMuc",
                        column: x => x.MaDanhMuc,
                        principalTable: "DanhMuc",
                        principalColumn: "MaDanhMuc",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ChiTietCombo",
                columns: table => new
                {
                    MaChiTietCombo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCombo = table.Column<int>(type: "int", nullable: false),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiTietCombo", x => x.MaChiTietCombo);
                    table.ForeignKey(
                        name: "FK_ChiTietCombo_Combo_MaCombo",
                        column: x => x.MaCombo,
                        principalTable: "Combo",
                        principalColumn: "MaCombo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChiTietCombo_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KichCoSanPham",
                columns: table => new
                {
                    MaKichCo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanPham = table.Column<int>(type: "int", nullable: false),
                    TenKichCo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GiaCongThem = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TrangThaiHoatDong = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KichCoSanPham", x => x.MaKichCo);
                    table.ForeignKey(
                        name: "FK_KichCoSanPham_SanPham_MaSanPham",
                        column: x => x.MaSanPham,
                        principalTable: "SanPham",
                        principalColumn: "MaSanPham",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DanhMuc",
                columns: new[] { "MaDanhMuc", "HinhAnh", "MaDanhMucCha", "MoTa", "TenDanhMuc", "ThuTuHienThi", "TrangThaiHoatDong" },
                values: new object[,]
                {
                    { 1, null, null, null, "Cà phê", 1, true },
                    { 2, null, null, null, "Trà", 2, true },
                    { 3, null, null, null, "Đá xay", 3, true },
                    { 4, null, null, null, "Bánh", 4, true },
                    { 5, null, null, null, "Khác", 5, true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombo_MaCombo",
                table: "ChiTietCombo",
                column: "MaCombo");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietCombo_MaSanPham",
                table: "ChiTietCombo",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_DanhMuc_MaDanhMucCha",
                table: "DanhMuc",
                column: "MaDanhMucCha");

            migrationBuilder.CreateIndex(
                name: "IX_KichCoSanPham_MaSanPham",
                table: "KichCoSanPham",
                column: "MaSanPham");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaDanhMuc",
                table: "SanPham",
                column: "MaDanhMuc");

            migrationBuilder.CreateIndex(
                name: "IX_SanPham_MaVach_SKU",
                table: "SanPham",
                column: "MaVach_SKU",
                unique: true,
                filter: "[MaVach_SKU] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChiTietCombo");

            migrationBuilder.DropTable(
                name: "KichCoSanPham");

            migrationBuilder.DropTable(
                name: "Combo");

            migrationBuilder.DropTable(
                name: "SanPham");

            migrationBuilder.DropTable(
                name: "DanhMuc");
        }
    }
}
