using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class CreateOrganizationsTableAndRetargeting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Companies_CompanyId",
                table: "Assignments");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CompanyId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Assignments");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                table: "Assignments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PuplicOrganizationsView",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContatsName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kskcode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OldOrganizationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DivisionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GroupCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GrounName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Root = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModificationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsNewMa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCsrcompany = table.Column<bool>(type: "bit", nullable: true),
                    IsHiden = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PuplicOrganizationsView", x => x.OrganizationId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_OrganizationId",
                table: "Assignments",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_PuplicOrganizationsView_OrganizationId",
                table: "Assignments",
                column: "OrganizationId",
                principalTable: "PuplicOrganizationsView",
                principalColumn: "OrganizationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_PuplicOrganizationsView_OrganizationId",
                table: "Assignments");

            migrationBuilder.DropTable(
                name: "PuplicOrganizationsView");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_OrganizationId",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Assignments");

            migrationBuilder.AddColumn<long>(
                name: "CompanyId",
                table: "Assignments",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Removed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Companies_PuplicEmployeeViews_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Companies_PuplicEmployeeViews_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "PuplicEmployeeViews",
                        principalColumn: "PositionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CompanyId",
                table: "Assignments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CreatedBy",
                table: "Companies",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_UpdatedBy",
                table: "Companies",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Companies_CompanyId",
                table: "Assignments",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
