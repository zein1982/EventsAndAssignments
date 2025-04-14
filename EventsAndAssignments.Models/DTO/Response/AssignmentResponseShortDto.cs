using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    public class AssignmentResponseShort : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Employee? ResponsibleLeader { get; set; }
        public DateTime? ExecutionDate { get; set; }
        public OrganizationResponseDto? Company { get; set; }
        public List<ResponsibleEmployee> ResponsibleEmployees { get; set; }
        public string? Comment { get; set; }
        public long? Status { get; set; }
        public long? ProtocolId { get; set; }
        public string? ProtocolInfo { get; set; }
        public bool IsArchived { get; set; }
        public ICollection<AssignmentFileResponseDto> Files { get; set; } = Array.Empty<AssignmentFileResponseDto>();
    }
}