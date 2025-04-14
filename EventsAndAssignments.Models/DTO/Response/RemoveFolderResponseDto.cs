namespace EventsAndAssignments.Models.DTO.Response
{
    public class RemoveFolderResponseDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsArchived { get; set; }
    }
}