using Microsoft.EntityFrameworkCore.Migrations;

namespace Under_the_Bay.API.Migrations
{
    public partial class Addmodelvalidations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PH",
                table: "Samples",
                newName: "pH");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "pH",
                table: "Samples",
                newName: "PH");
        }
    }
}
