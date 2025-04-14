using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class ManyAdminsToFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeProtocolFolder",
                columns: table => new
                {
                    AllowedEmployeesNavigationPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProtocolFoldersAllowedEmployeesNavigationId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeProtocolFolder", x => new { x.AllowedEmployeesNavigationPositionId, x.ProtocolFoldersAllowedEmployeesNavigationId });
                    table.ForeignKey(
                        name: "FK_EmployeeProtocolFolder_ProtocolFolders_ProtocolFoldersAllowedEmployeesNavigationId",
                        column: x => x.ProtocolFoldersAllowedEmployeesNavigationId,
                        principalTable: "ProtocolFolders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeProtocolFolder_PuplicEmployeeViews_AllowedEmployeesNavigationPositionId",
                        column: x => x.AllowedEmployeesNavigationPositionId,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeProtocolFolder_ProtocolFoldersAllowedEmployeesNavigationId",
                table: "EmployeeProtocolFolder",
                column: "ProtocolFoldersAllowedEmployeesNavigationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeProtocolFolder");
        }
    }
}
