using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    public class AssignmentResponse : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public Employee? Author { get; set; }
        public DateTime ExecutionDate { get; set; }
        public OrganizationResponseDto? Company { get; set; }
        public long GroupId { get; set; }
        public string EventDirection { get; set; } = string.Empty;
        public IList<ResponsibleResponse> ResponsibleLeaders { get; set; } = Array.Empty<ResponsibleResponse>();
        public IList<ResponsibleResponse> ResponsibleExecutors { get; set; } = Array.Empty<ResponsibleResponse>();
        public IList<ResponsibleResponse> ResponsibleInspectors { get; set; } = Array.Empty<ResponsibleResponse>();
        public string? Description { get; set; }
        public long? Status { get; set; }
        public int Subversion { get; set; }
        public int Version { get; set; }
        public long? ProtocolId { get; set; }
        public string? ProtocolInfo { get; set; }
        public bool IsArchived { get; set; }
        public IList<AssignmentFileResponseDto>? Files { get; set; } = Array.Empty<AssignmentFileResponseDto>();
        public bool UserCanAddComment { get; set; }
        public Guid? ProtocolCreatedBy { get; set; }
        public Guid? FolderCreatedBy { get; set; }
        public ICollection<Employee>? AllowedEmployeesNavigation { get; set; }
    }
}