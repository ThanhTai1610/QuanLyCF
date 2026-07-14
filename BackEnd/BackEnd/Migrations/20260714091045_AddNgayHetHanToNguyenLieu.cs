using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddNgayHetHanToNguyenLieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NgayHetHan",
                table: "NguyenLieu",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhanLoai",
                table: "NguyenLieu",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Nguyên liệu thô");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NgayHetHan",
                table: "NguyenLieu");

            migrationBuilder.DropColumn(
                name: "PhanLoai",
                table: "NguyenLieu");
        }
    }
}
