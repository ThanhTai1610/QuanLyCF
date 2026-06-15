using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhuVucBan",
                columns: table => new
                {
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenKhuVuc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhuThu = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhuVucBan", x => x.MaKhuVuc);
                });

            migrationBuilder.CreateTable(
                name: "Ban",
                columns: table => new
                {
                    MaBan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKhuVuc = table.Column<int>(type: "int", nullable: false),
                    TenBan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SucChua = table.Column<int>(type: "int", nullable: true),
                    MaQRHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ban", x => x.MaBan);
                    table.ForeignKey(
                        name: "FK_Ban_KhuVucBan_MaKhuVuc",
                        column: x => x.MaKhuVuc,
                        principalTable: "KhuVucBan",
                        principalColumn: "MaKhuVuc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Quyen",
                columns: new[] { "MaQuyen", "MaCode", "Nhom", "TenQuyen" },
                values: new object[,]
                {
                    { 17, "BAN_XEM", "Ban", "Xem bàn" },
                    { 18, "BAN_QUANLY", "Ban", "Quản lý bàn" }
                });

            migrationBuilder.InsertData(
                table: "VaiTro_Quyen",
                columns: new[] { "MaQuyen", "MaVaiTro" },
                values: new object[,]
                {
                    { 17, 1 },
                    { 18, 1 },
                    { 17, 3 },
                    { 17, 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ban_MaKhuVuc",
                table: "Ban",
                column: "MaKhuVuc");

            migrationBuilder.CreateIndex(
                name: "IX_Ban_MaQRHash",
                table: "Ban",
                column: "MaQRHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ban_TenBan",
                table: "Ban",
                column: "TenBan",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ban");

            migrationBuilder.DropTable(
                name: "KhuVucBan");

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 17, 1 });

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 18, 1 });

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 17, 3 });

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 17, 4 });

            migrationBuilder.DeleteData(
                table: "Quyen",
                keyColumn: "MaQuyen",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Quyen",
                keyColumn: "MaQuyen",
                keyValue: 18);
        }
    }
}
