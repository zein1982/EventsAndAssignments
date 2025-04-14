using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    public class ResponsibleResponse
    {
        public Employee? Employee { get; set; }
        public DateTime? ExecutionDate { get; set; }
    }
}