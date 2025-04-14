namespace EventsAndAssignments.Models.DTO.Response
{
    public class AssignmentVersionResponse
    {
        public long GroupId { get; set; }
        public int Version { get; set; }
        public int Subversion { get; set; }
        public string? CurrentStatus { get; set; }
    }
}