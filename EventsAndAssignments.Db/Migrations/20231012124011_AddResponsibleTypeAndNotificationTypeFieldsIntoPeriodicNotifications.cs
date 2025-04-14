using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsibleTypeAndNotificationTypeFieldsIntoPeriodicNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NotificationType",
                table: "PeriodicNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleType",
                table: "PeriodicNotifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("UPDATE PeriodicNotifications SET ResponsibleType = 1 WHERE ResponsibleType = 0 AND Message LIKE N'%руководителя%'");
            migrationBuilder.Sql("UPDATE PeriodicNotifications SET ResponsibleType = 2 WHERE ResponsibleType  = 0 AND Message LIKE N'%исполнителя%'");
            migrationBuilder.Sql("UPDATE PeriodicNotifications SET ResponsibleType = 3 WHERE ResponsibleType = 0 AND Message LIKE N'%контролера%'");
            migrationBuilder.Sql("UPDATE PeriodicNotifications SET NotificationType = 0 WHERE NotificationType IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "PeriodicNotifications");

            migrationBuilder.DropColumn(
                name: "ResponsibleType",
                table: "PeriodicNotifications");
        }
    }
}