using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventsAndAssignments.Db.Migrations
{
    public partial class CompletionDateAdd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ass
                SET ass.CompletionDate = hist.Created
                FROM Assignments AS ass
                INNER JOIN (
                    SELECT AssignmentId, MAX(Created) AS MaxDate
                    FROM AssignmentHistories
                    WHERE ToStatus = 7
                    GROUP BY AssignmentId
                ) AS t ON ass.Id = t.AssignmentId
                INNER JOIN AssignmentHistories AS hist ON t.AssignmentId = hist.AssignmentId AND t.MaxDate = hist.Created
                WHERE ass.StatusId = 7 AND hist.ToStatus = 7;
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "Assignments");
        }
    }
}