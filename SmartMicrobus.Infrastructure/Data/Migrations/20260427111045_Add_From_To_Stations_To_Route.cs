using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_From_To_Stations_To_Route : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_StationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromAr_ToAr",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromEn_ToEn",
                table: "Routes");

            migrationBuilder.AlterColumn<Guid>(
                name: "StationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "FromStationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ToStationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromStationId_ToStationId",
                table: "Routes",
                columns: new[] { "FromStationId", "ToStationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_ToStationId",
                table: "Routes",
                column: "ToStationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_FromStationId",
                table: "Routes",
                column: "FromStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_StationId",
                table: "Routes",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_ToStationId",
                table: "Routes",
                column: "ToStationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_FromStationId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_StationId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Stations_ToStationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromStationId_ToStationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_ToStationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "FromStationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "ToStationId",
                table: "Routes");

            migrationBuilder.AlterColumn<Guid>(
                name: "StationId",
                table: "Routes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromAr_ToAr",
                table: "Routes",
                columns: new[] { "FromAr", "ToAr" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromEn_ToEn",
                table: "Routes",
                columns: new[] { "FromEn", "ToEn" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Stations_StationId",
                table: "Routes",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
