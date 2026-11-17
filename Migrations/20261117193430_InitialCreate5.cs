using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniYume.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Roles",
                table: "Users",
                type: "text[]",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Animes",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Roles",
                table: "Users");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReleaseDate",
                table: "Animes",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
