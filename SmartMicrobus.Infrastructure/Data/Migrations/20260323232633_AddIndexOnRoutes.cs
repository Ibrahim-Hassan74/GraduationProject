using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexOnRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_FromAr_ToAr",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromEn_ToEn",
                table: "Routes");
        }
    }
}
