using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartMicrobus.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIndexerFromQueueItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QueueItems_QueueId_Position",
                table: "QueueItems");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_QueueId",
                table: "QueueItems",
                column: "QueueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_QueueItems_QueueId",
                table: "QueueItems");

            migrationBuilder.CreateIndex(
                name: "IX_QueueItems_QueueId_Position",
                table: "QueueItems",
                columns: new[] { "QueueId", "Position" },
                unique: true);
        }
    }
}
