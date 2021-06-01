using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Under_the_Bay.API.Migrations
{
    public partial class Addsampledomainmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Samples",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SampleDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    WaterTemperature = table.Column<float>(type: "real", nullable: false),
                    DissolvedOxygen = table.Column<float>(type: "real", nullable: false),
                    DissolvedOxygenSaturation = table.Column<float>(type: "real", nullable: false),
                    Salinity = table.Column<float>(type: "real", nullable: false),
                    PH = table.Column<float>(type: "real", nullable: false),
                    Turbidity = table.Column<float>(type: "real", nullable: false),
                    Chlorophyll = table.Column<float>(type: "real", nullable: false),
                    BlueGreenAlgae = table.Column<float>(type: "real", nullable: false),
                    StationId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Samples", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Samples_Stations_StationId",
                        column: x => x.StationId,
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Samples_StationId",
                table: "Samples",
                column: "StationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Samples");
        }
    }
}
