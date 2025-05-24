using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace quangcao.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_ApplicationUserId",
                table: "AnhBiaTrangs");

            migrationBuilder.DropForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_UserId",
                table: "AnhBiaTrangs");

            migrationBuilder.DropIndex(
                name: "IX_AnhBiaTrangs_ApplicationUserId",
                table: "AnhBiaTrangs");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "AnhBiaTrangs");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_UserId",
                table: "AnhBiaTrangs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_UserId",
                table: "AnhBiaTrangs");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "AnhBiaTrangs",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnhBiaTrangs_ApplicationUserId",
                table: "AnhBiaTrangs",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_ApplicationUserId",
                table: "AnhBiaTrangs",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnhBiaTrangs_AspNetUsers_UserId",
                table: "AnhBiaTrangs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
