using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMicrobusColorAndModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Microbuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Microbuses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Microbuses");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Microbuses");
        }
    }
}
