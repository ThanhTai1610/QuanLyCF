using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddBankSettingsSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CaiDatHeThong",
                columns: new[] { "MaCaiDat", "GiaTriCaiDat", "KhoaCaiDat", "MoTa", "NhomCaiDat", "ThoiGianCapNhat" },
                values: new object[,]
                {
                    { 10, "MB", "NGAN_HANG_ID", "Mã ngân hàng nhận chuyển khoản (MB, VCB...)", "THANH_TOAN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, "19035282928014", "NGAN_HANG_STK", "Số tài khoản ngân hàng nhận chuyển khoản", "THANH_TOAN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, "CONG TY BREWMANAGER", "NGAN_HANG_TEN", "Tên chủ tài khoản ngân hàng nhận chuyển khoản", "THANH_TOAN", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CaiDatHeThong",
                keyColumn: "MaCaiDat",
                keyValue: 12);
        }
    }
}
