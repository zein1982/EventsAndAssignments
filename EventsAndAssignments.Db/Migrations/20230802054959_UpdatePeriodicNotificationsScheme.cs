using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePeriodicNotificationsScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodicNotifications_Protocols_ProtocolId",
                table: "PeriodicNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_PeriodicNotifications_PuplicEmployeeViews_RecipientPositionId",
                table: "PeriodicNotifications");

            migrationBuilder.DropIndex(
                name: "IX_PeriodicNotifications_ProtocolId",
                table: "PeriodicNotifications");

            migrationBuilder.DropColumn(
                name: "ProtocolId",
                table: "PeriodicNotifications");

            migrationBuilder.RenameColumn(
                name: "Template",
                table: "PeriodicNotifications",
                newName: "Message");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecipientPositionId",
                table: "PeriodicNotifications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExecutionDate",
                table: "PeriodicNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SendDate",
                table: "PeriodicNotifications",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "PeriodicNotifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodicNotifications_PuplicEmployeeViews_RecipientPositionId",
                table: "PeriodicNotifications",
                column: "RecipientPositionId",
                principalTable: "PuplicEmployeeViews",
                principalColumn: "PositionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeriodicNotifications_PuplicEmployeeViews_RecipientPositionId",
                table: "PeriodicNotifications");

            migrationBuilder.DropColumn(
                name: "ExecutionDate",
                table: "PeriodicNotifications");

            migrationBuilder.DropColumn(
                name: "SendDate",
                table: "PeriodicNotifications");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "PeriodicNotifications");

            migrationBuilder.RenameColumn(
                name: "Message",
                table: "PeriodicNotifications",
                newName: "Template");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecipientPositionId",
                table: "PeriodicNotifications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProtocolId",
                table: "PeriodicNotifications",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeriodicNotifications_ProtocolId",
                table: "PeriodicNotifications",
                column: "ProtocolId");

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodicNotifications_Protocols_ProtocolId",
                table: "PeriodicNotifications",
                column: "ProtocolId",
                principalTable: "Protocols",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeriodicNotifications_PuplicEmployeeViews_RecipientPositionId",
                table: "PeriodicNotifications",
                column: "RecipientPositionId",
                principalTable: "PuplicEmployeeViews",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
