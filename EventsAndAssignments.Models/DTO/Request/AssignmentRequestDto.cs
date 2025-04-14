using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class AssignmentRequestDto : BaseDTO
    {
        [DefaultValue(null)]
        public Guid? CompanyId { get; set; }

        [DefaultValue(null)]
        public Guid? AuthorId { get; set; }

        [Required]
        public DateTime ExecutionDate { get; set; }

        public IList<ResponsibleRequest?> ResponsibleLeaders { get; set; } = Array.Empty<ResponsibleRequest>();
        public IList<ResponsibleRequest?> ResponsibleExecutors { get; set; } = Array.Empty<ResponsibleRequest>();
        public IList<ResponsibleRequest?> ResponsibleInspectors { get; set; } = Array.Empty<ResponsibleRequest>();
        public string? Description { get; set; }

        [Required]
        [Range(1, double.MaxValue)]
        public long ProtocolId { get; set; }

        [DefaultValue(1)]
        public long? StatusId { get; set; }

        [DefaultValue(0)]
        [Range(0, int.MaxValue)]
        public int Subversion { get; set; }

        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int Version { get; set; }

        [DefaultValue(false)]
        public bool? NeedToReturnForRevision { get; set; }
    }
}