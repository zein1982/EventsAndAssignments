namespace EventsAndAssignments.Models.DTO.Request
{
    public class AssignmentVersionRequestDTO
    {
        public long GroupId { get; set; }
        public int Version { get; set; }
        public int Subversion { get; set; }
    }
}