using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameFavoriteRoutesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoute_Passangers_PassengerId",
                table: "FavoriteRoute");

            migrationBuilder.DropForeignKey(
                name: "FK_FavoriteRoute_Routes_RouteId",
                table: "FavoriteRoute");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
