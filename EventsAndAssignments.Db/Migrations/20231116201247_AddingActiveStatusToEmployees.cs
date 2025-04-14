using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    /// <inheritdoc />
    public partial class AddingActiveStatusToEmployees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "PuplicEmployeeViews",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "PuplicEmployeeViews");
        }
    }
}
