using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments_DataTransfer.Migrations
{
    public partial class AddPuplicorganizationview : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PuplicOrganizationsViews",
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
                    table.PrimaryKey("PK_PuplicOrganizationsViews", x => x.OrganizationId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PuplicOrganizationsViews");
        }
    }
}
