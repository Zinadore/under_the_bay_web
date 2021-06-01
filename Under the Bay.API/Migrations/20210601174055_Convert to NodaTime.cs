using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace Under_the_Bay.API.Migrations
{
    public partial class ConverttoNodaTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Instant>(
                name: "LastUpdate",
                table: "Stations",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<Instant>(
                name: "SampleDate",
                table: "Samples",
                type: "timestamp",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastUpdate",
                table: "Stations",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(Instant),
                oldType: "timestamp");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "SampleDate",
                table: "Samples",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(Instant),
                oldType: "timestamp");
        }
    }
}
