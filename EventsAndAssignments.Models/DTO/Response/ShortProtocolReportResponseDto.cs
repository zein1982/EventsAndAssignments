using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    public class ShortProtocolReportResponseDto
    {
        public long Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Employee? ResponsibleLeader { get; set; }
    }
}