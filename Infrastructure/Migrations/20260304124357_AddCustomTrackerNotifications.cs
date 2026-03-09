using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomTrackerNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomTrackerId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CustomTrackerId",
                table: "Notifications",
                column: "CustomTrackerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_CustomTrackers_CustomTrackerId",
                table: "Notifications",
                column: "CustomTrackerId",
                principalTable: "CustomTrackers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_CustomTrackers_CustomTrackerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_CustomTrackerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CustomTrackerId",
                table: "Notifications");
        }
    }
}
