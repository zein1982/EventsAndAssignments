using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBehaviorWhenDeletePeriodicNotificationToSetNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_PeriodicNotifications_PeriodicNotificationId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PeriodicNotificationId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PeriodicNotificationId",
                table: "Notifications",
                column: "PeriodicNotificationId",
                unique: true,
                filter: "[PeriodicNotificationId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_PeriodicNotifications_PeriodicNotificationId",
                table: "Notifications",
                column: "PeriodicNotificationId",
                principalTable: "PeriodicNotifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_PeriodicNotifications_PeriodicNotificationId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_PeriodicNotificationId",
                table: "Notifications");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_PeriodicNotificationId",
                table: "Notifications",
                column: "PeriodicNotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_PeriodicNotifications_PeriodicNotificationId",
                table: "Notifications",
                column: "PeriodicNotificationId",
                principalTable: "PeriodicNotifications",
                principalColumn: "Id");
        }
    }
}
