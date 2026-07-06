using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApDungKhungGio",
                table: "DanhMuc",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioBatDau",
                table: "DanhMuc",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioKetThuc",
                table: "DanhMuc",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LoaiDanhMuc",
                table: "DanhMuc",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 1,
                columns: new[] { "ApDungKhungGio", "GioBatDau", "GioKetThuc", "LoaiDanhMuc" },
                values: new object[] { false, null, null, "MAIN" });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 2,
                columns: new[] { "ApDungKhungGio", "GioBatDau", "GioKetThuc", "LoaiDanhMuc" },
                values: new object[] { false, null, null, "MAIN" });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 3,
                columns: new[] { "ApDungKhungGio", "GioBatDau", "GioKetThuc", "LoaiDanhMuc" },
                values: new object[] { false, null, null, "MAIN" });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 4,
                columns: new[] { "ApDungKhungGio", "GioBatDau", "GioKetThuc", "LoaiDanhMuc" },
                values: new object[] { false, null, null, "MAIN" });

            migrationBuilder.UpdateData(
                table: "DanhMuc",
                keyColumn: "MaDanhMuc",
                keyValue: 5,
                columns: new[] { "ApDungKhungGio", "GioBatDau", "GioKetThuc", "LoaiDanhMuc" },
                values: new object[] { false, null, null, "MAIN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApDungKhungGio",
                table: "DanhMuc");

            migrationBuilder.DropColumn(
                name: "GioBatDau",
                table: "DanhMuc");

            migrationBuilder.DropColumn(
                name: "GioKetThuc",
                table: "DanhMuc");

            migrationBuilder.DropColumn(
                name: "LoaiDanhMuc",
                table: "DanhMuc");
        }
    }
}
