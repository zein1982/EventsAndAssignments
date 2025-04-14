namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Уведомление
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Наименование протокола
        /// </summary>
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Recipient { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public DateTime SendDate { get; set; }
        public long? PeriodicNotificationId { get; set; }
        public PeriodicNotification? PeriodicNotification { get; set; }
    }
}