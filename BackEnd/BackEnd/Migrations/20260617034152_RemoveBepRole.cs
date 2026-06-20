using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBepRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 7, 5 });

            migrationBuilder.DeleteData(
                table: "VaiTro_Quyen",
                keyColumns: new[] { "MaQuyen", "MaVaiTro" },
                keyValues: new object[] { 9, 5 });

            migrationBuilder.DeleteData(
                table: "VaiTro",
                keyColumn: "MaVaiTro",
                keyValue: 5);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VaiTro",
                columns: new[] { "MaVaiTro", "LaVaiTroHeThong", "MoTa", "TenVaiTro" },
                values: new object[] { 5, true, "Màn hình bếp", "Bếp" });

            migrationBuilder.InsertData(
                table: "VaiTro_Quyen",
                columns: new[] { "MaQuyen", "MaVaiTro" },
                values: new object[,]
                {
                    { 7, 5 },
                    { 9, 5 }
                });
        }
    }
}
