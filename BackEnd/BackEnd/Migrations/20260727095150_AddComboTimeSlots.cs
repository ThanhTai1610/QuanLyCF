using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddComboTimeSlots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ApDungKhungGio",
                table: "Combo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioBatDau",
                table: "Combo",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "GioKetThuc",
                table: "Combo",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApDungKhungGio",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "GioBatDau",
                table: "Combo");

            migrationBuilder.DropColumn(
                name: "GioKetThuc",
                table: "Combo");
        }
    }
}
