using Microsoft.EntityFrameworkCore.Migrations;

namespace Lapka.API.DAL.Migrations
{
    public partial class issterilized_haspassport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Passport",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "IsCastrated",
                table: "Animals",
                newName: "IsSterilized");

            migrationBuilder.AddColumn<bool>(
                name: "HasPassport",
                table: "Animals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasPassport",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "IsSterilized",
                table: "Animals",
                newName: "IsCastrated");

            migrationBuilder.AddColumn<string>(
                name: "Passport",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
