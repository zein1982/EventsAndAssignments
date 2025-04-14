using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class AssignmentShortRequestDto : BaseDTO
    {
        public bool IsChecked { get; set; }
        public string? Description { get; set; }

        [DefaultValue(null)]
        public Guid? ResponsibleLeaderId { get; set; }

        [DefaultValue(null)]
        public Guid? CompanyId { get; set; }

        [Range(1, double.MaxValue)]
        public long ProtocolId { get; set; }

        public DateTime? ExecutionDate { get; set; }
        public string? Comment { get; set; }
    }
}