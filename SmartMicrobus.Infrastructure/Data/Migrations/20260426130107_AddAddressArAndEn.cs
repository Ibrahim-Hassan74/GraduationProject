using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressArAndEn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Stations",
                newName: "AddressEn");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Stations",
                type: "geography",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography");

            migrationBuilder.AddColumn<string>(
                name: "AddressAr",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressAr",
                table: "Stations");

            migrationBuilder.RenameColumn(
                name: "AddressEn",
                table: "Stations",
                newName: "Address");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Stations",
                type: "geography",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography",
                oldNullable: true);
        }
    }
}
