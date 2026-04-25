using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Fix_All_Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Microbuses_Routes_RouteId1",
                table: "Microbuses");

            migrationBuilder.DropForeignKey(
                name: "FK_Queues_Routes_RouteId1",
                table: "Queues");

            migrationBuilder.DropForeignKey(
                name: "FK_Queues_Stations_StationId1",
                table: "Queues");

            migrationBuilder.DropIndex(
                name: "IX_Queues_RouteId1",
                table: "Queues");

            migrationBuilder.DropIndex(
                name: "IX_Queues_StationId1",
                table: "Queues");

            migrationBuilder.DropIndex(
                name: "IX_Microbuses_RouteId1",
                table: "Microbuses");

            migrationBuilder.DropColumn(
                name: "RouteId1",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "StationId1",
                table: "Queues");

            migrationBuilder.DropColumn(
                name: "RouteId1",
                table: "Microbuses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RouteId1",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StationId1",
                table: "Queues",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RouteId1",
                table: "Microbuses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Queues_RouteId1",
                table: "Queues",
                column: "RouteId1");

            migrationBuilder.CreateIndex(
                name: "IX_Queues_StationId1",
                table: "Queues",
                column: "StationId1");

            migrationBuilder.CreateIndex(
                name: "IX_Microbuses_RouteId1",
                table: "Microbuses",
                column: "RouteId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Microbuses_Routes_RouteId1",
                table: "Microbuses",
                column: "RouteId1",
                principalTable: "Routes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Queues_Routes_RouteId1",
                table: "Queues",
                column: "RouteId1",
                principalTable: "Routes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Queues_Stations_StationId1",
                table: "Queues",
                column: "StationId1",
                principalTable: "Stations",
                principalColumn: "Id");
        }
    }
}
