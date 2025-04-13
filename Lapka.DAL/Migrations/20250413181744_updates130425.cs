using Microsoft.EntityFrameworkCore.Migrations;

namespace Lapka.API.DAL.Migrations
{
    public partial class updates130425 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Shelters",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Shelters_UserId",
                table: "Shelters",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shelters_AspNetUsers_UserId",
                table: "Shelters",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shelters_AspNetUsers_UserId",
                table: "Shelters");

            migrationBuilder.DropIndex(
                name: "IX_Shelters_UserId",
                table: "Shelters");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shelters");
        }
    }
}
