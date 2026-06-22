using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddMaintenanceSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 2,
                column: "GiaTriCaiDat",
                value: "123 Nguyễn Huệ, Quận 1, TP.HCM");

            migrationBuilder.InsertData(
                table: "CaiDatHeThong",
                columns: new[] { "MaCaiDat", "GiaTriCaiDat", "KhoaCaiDat", "MoTa", "NhomCaiDat", "ThoiGianCapNhat" },
                values: new object[,]
                {
                    { 6, "false", "CHE_DO_BAO_TRI", "Chế độ bảo trì hệ thống", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "Hệ thống đang bảo trì để nâng cấp định kỳ. Vui lòng quay lại sau.", "THONG_DIEP_BAO_TRI", "Thông điệp bảo trì", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "0909 123 456", "SO_DIEN_THOAI", "Số điện thoại quán", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "Quán cà phê đặc sản với không gian ấm cúng. Phục vụ cà phê pha máy, trà, bánh ngọt và các loại đồ uống đá xay.", "MO_TA_QUAN", "Mô tả quán", "CHUNG", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 9);

            migrationBuilder.UpdateData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 2,
                column: "GiaTriCaiDat",
                value: "");
        }
    }
}
