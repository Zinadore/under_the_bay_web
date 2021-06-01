using Microsoft.EntityFrameworkCore.Migrations;

namespace Under_the_Bay.API.Migrations
{
    public partial class Addsampledepthtosamplemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "SampleDepth",
                table: "Samples",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SampleDepth",
                table: "Samples");
        }
    }
}
