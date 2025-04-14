namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Запланированные повторяющиеся уведомления
    /// </summary>
    public class PeriodicNotification
    {
        /// <summary>
        /// Идентификатор уведомления
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Заголовок уведомления
        /// </summary>
        public string? Subject { get; set; }

        /// <summary>
        /// Текст основного сообщения
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Дата отправки уведомления
        /// </summary>
        public DateTime SendDate { get; set; }

        /// <summary>
        /// Дата исполнения (deadline)
        /// </summary>
        public DateTime ExecutionDate { get; set; }

        /// <summary>
        /// Тип уведомления. Мапиться на enum NotificationType
        /// </summary>
        public int NotificationType { get; set; }

        /// <summary>
        /// Тип ответственного для которого создано уведомление. Мапиться на enum ResponsibleType
        /// </summary>
        public int ResponsibleType { get; set; }

        /// <summary>
        /// Получатель уведомления
        /// </summary>
        public Employee? Recipient { get; set; }

        /// <summary>
        /// Идентификатор получателя уведомления
        /// </summary>
        public Guid? RecipientPositionId { get; set; }

        /// <summary>
        /// Поручение в рамках котрого создано уведомление
        /// </summary>
        public Assignment? Assignment { get; set; }

        /// <summary>
        /// Идентификатор поручения в рамках котрого создано уведомление
        /// </summary>
        public long? AssignmentId { get; set; }
    }
}