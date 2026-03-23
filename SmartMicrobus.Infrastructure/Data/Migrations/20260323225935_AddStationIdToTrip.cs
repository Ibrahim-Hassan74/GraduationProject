using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStationIdToTrip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "StationId",
                table: "Trips",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.CreateIndex(
                name: "IX_Trips_StationId",
                table: "Trips",
                column: "StationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trips_Stations_StationId",
                table: "Trips");

            migrationBuilder.DropIndex(
                name: "IX_Trips_StationId",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "StationId",
                table: "Trips");
        }
    }
}
