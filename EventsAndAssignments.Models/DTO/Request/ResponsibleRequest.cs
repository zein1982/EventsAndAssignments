using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class ResponsibleRequest
    {
        [Required]
        public Guid EmployeePositionId { get; set; }

        //[Required]
        public DateTime? ExecutionDate { get; set; }
    }
}