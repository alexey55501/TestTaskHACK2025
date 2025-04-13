using Microsoft.EntityFrameworkCore.Migrations;

namespace Lapka.API.DAL.Migrations
{
    public partial class animalupdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "War",
                table: "Animals",
                newName: "HealthDescription");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HealthDescription",
                table: "Animals",
                newName: "War");
        }
    }
}
