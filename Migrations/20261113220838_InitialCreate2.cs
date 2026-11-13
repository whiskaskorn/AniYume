using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AniYume.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CodeForConfirmEmail",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isExsistsEmail",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeForConfirmEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isExsistsEmail",
                table: "Users");
        }
    }
}
