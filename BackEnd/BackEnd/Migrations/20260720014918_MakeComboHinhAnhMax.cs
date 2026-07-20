using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class MakeComboHinhAnhMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Combo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Quyen",
                columns: new[] { "MaQuyen", "MaCode", "Nhom", "TenQuyen" },
                values: new object[] { 19, "HOADON_XEM", "HoaDon", "Xem hoá đơn" });

            migrationBuilder.InsertData(
                table: "VaiTro_Quyen",
                columns: new[] { "MaQuyen", "MaVaiTro" },
                values: new object[,]
                {
                    { 19, 1 },
                    { 19, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 19, 1 });

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 19, 3 });

            migrationBuilder.DeleteData(
                table: "Quyen",
                keyColumn: "MaQuyen",
                keyValue: 19);

            migrationBuilder.AlterColumn<string>(
                name: "HinhAnh",
                table: "Combo",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
