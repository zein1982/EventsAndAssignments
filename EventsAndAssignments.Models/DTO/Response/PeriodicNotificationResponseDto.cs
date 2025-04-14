namespace EventsAndAssignments.Models.DTO.Response
{
    public class PeriodicNotificationResponseDto
    {
        public long Id { get; set; }
        public string? Subject { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SendDate { get; set; }
        public DateTime ExecutionDate { get; set; }
        public string? Recipient { get; set; }
        public int NotificationType { get; set; }
    }
}