using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationBetweenManagerAndStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Managers_StationId",
                table: "Managers");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_StationId",
                table: "Managers",
                column: "StationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Managers_StationId",
                table: "Managers");

            migrationBuilder.CreateIndex(
                name: "IX_Managers_StationId",
                table: "Managers",
                column: "StationId",
                unique: true);
        }
    }
}
