using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddTableLockPinSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaPinSession",
                table: "Ban",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoaiDatBan",
                table: "Ban",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ThoiGianKhoaHetHan",
                table: "Ban",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaPinSession",
                table: "Ban");

            migrationBuilder.DropColumn(
                name: "SoDienThoaiDatBan",
                table: "Ban");

            migrationBuilder.DropColumn(
                name: "ThoiGianKhoaHetHan",
                table: "Ban");
        }
    }
}
