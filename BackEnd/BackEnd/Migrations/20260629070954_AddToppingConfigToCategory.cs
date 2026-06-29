using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddToppingConfigToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LaNhomKichThuoc",
                table: "DanhMuc",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ToiDaChon",
                table: "DanhMuc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ToiThieuChon",
                table: "DanhMuc",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 1,
                columns: new[] { "LaNhomKichThuoc", "ToiDaChon", "ToiThieuChon" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 2,
                columns: new[] { "LaNhomKichThuoc", "ToiDaChon", "ToiThieuChon" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 3,
                columns: new[] { "LaNhomKichThuoc", "ToiDaChon", "ToiThieuChon" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 4,
                columns: new[] { "LaNhomKichThuoc", "ToiDaChon", "ToiThieuChon" },
                values: new object[] { false, null, null });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 5,
                columns: new[] { "LaNhomKichThuoc", "ToiDaChon", "ToiThieuChon" },
                values: new object[] { false, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaNhomKichThuoc",
                table: "DanhMuc");

            migrationBuilder.DropColumn(
                name: "ToiDaChon",
                table: "DanhMuc");

            migrationBuilder.DropColumn(
                name: "ToiThieuChon",
                table: "DanhMuc");
        }
    }
}
