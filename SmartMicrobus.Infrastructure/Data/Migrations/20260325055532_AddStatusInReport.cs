using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusInReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DriverReports",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "DriverReports");
        }
    }
}
