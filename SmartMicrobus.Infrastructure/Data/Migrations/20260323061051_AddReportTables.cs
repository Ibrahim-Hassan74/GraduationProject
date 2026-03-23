using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReportTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoutes_Passangers_PassengerId",
                table: "FavoriteRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoutes_Routes_RouteId",
                table: "FavoriteRoutes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteRoutes",
                table: "FavoriteRoutes");

            migrationBuilder.RenameTable(
                name: "FavoriteRoutes",
                newName: "FavoriteRoute");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteRoutes_RouteId",
                table: "FavoriteRoute",
                newName: "IX_FavoriteRoute_RouteId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteRoutes_PassengerId_RouteId",
                table: "FavoriteRoute",
                newName: "IX_FavoriteRoute_PassengerId_RouteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteRoute",
                table: "FavoriteRoute",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "DriverReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DriverId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverReports_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DriverReports_Passangers_PassengerId",
                        column: x => x.PassengerId,
                        principalTable: "Passangers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportReasons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DriverReportReasons",
                columns: table => new
                {
                    DriverReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportReasonId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverReportReasons", x => new { x.DriverReportId, x.ReportReasonId });
                    table.ForeignKey(
                        name: "FK_DriverReportReasons_DriverReports_DriverReportId",
                        column: x => x.DriverReportId,
                        principalTable: "DriverReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverReportReasons_ReportReasons_ReportReasonId",
                        column: x => x.ReportReasonId,
                        principalTable: "ReportReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverReportReasons_ReportReasonId",
                table: "DriverReportReasons",
                column: "ReportReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverReports_DriverId",
                table: "DriverReports",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverReports_PassengerId",
                table: "DriverReports",
                column: "PassengerId");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRoute_Passangers_PassengerId",
                table: "FavoriteRoute",
                column: "PassengerId",
                principalTable: "Passangers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRoute_Routes_RouteId",
                table: "FavoriteRoute",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoute_Passangers_PassengerId",
                table: "FavoriteRoute");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoute_Routes_RouteId",
                table: "FavoriteRoute");

            migrationBuilder.DropTable(
                name: "DriverReportReasons");

            migrationBuilder.DropTable(
                name: "DriverReports");

            migrationBuilder.DropTable(
                name: "ReportReasons");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FavoriteRoute",
                table: "FavoriteRoute");

            migrationBuilder.RenameTable(
                name: "FavoriteRoute",
                newName: "FavoriteRoutes");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteRoute_RouteId",
                table: "FavoriteRoutes",
                newName: "IX_FavoriteRoutes_RouteId");

            migrationBuilder.RenameIndex(
                name: "IX_FavoriteRoute_PassengerId_RouteId",
                table: "FavoriteRoutes",
                newName: "IX_FavoriteRoutes_PassengerId_RouteId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FavoriteRoutes",
                table: "FavoriteRoutes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRoutes_Passangers_PassengerId",
                table: "FavoriteRoutes",
                column: "PassengerId",
                principalTable: "Passangers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FavoriteRoutes_Routes_RouteId",
                table: "FavoriteRoutes",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
