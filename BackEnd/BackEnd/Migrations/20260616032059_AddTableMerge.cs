using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddTableMerge : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaBanChinh",
                table: "Ban",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ban_MaBanChinh",
                table: "Ban",
                column: "MaBanChinh");

            migrationBuilder.AddForeignKey(
                name: "FK_Ban_Ban_MaBanChinh",
                table: "Ban",
                column: "MaBanChinh",
                principalTable: "Ban",
                principalColumn: "MaBan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ban_Ban_MaBanChinh",
                table: "Ban");

            migrationBuilder.DropIndex(
                name: "IX_Ban_MaBanChinh",
                table: "Ban");

            migrationBuilder.DropColumn(
                name: "MaBanChinh",
                table: "Ban");
        }
    }
}
