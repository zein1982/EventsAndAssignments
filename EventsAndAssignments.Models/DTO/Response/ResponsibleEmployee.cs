namespace EventsAndAssignments.Models.DTO.Response
{
    public class ResponsibleEmployee
    {
        public string? EmployeeName { get; set; }
        public string Position { get; set; }

        public ResponsibleEmployee(string employeeName, string position)
        {
            EmployeeName = employeeName;
            Position = position;
        }
    }
}
