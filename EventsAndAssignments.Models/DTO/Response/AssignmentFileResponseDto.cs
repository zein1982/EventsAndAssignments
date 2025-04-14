using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    public class AssignmentFileResponseDto
    {
        public long? Id { get; set; }
        public string Name { get; set; } = "file";
        public DateTime Created { get; set; }
        public Employee? CreatedBy { get; set; }
    }
}