namespace EventsAndAssignments.Models.DTO.Response
{
    public class RemoveFileDtoResponse
    {
        public long Id { get; set; }
        public string OriginName { get; set; } = string.Empty;
        public DateTime Removed { get; set; }
        //public Guid CreatedBy { get; set; }
    }
}