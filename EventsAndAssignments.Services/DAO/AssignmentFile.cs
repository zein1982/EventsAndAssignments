namespace EventsAndAssignments.Services.DAO
{
    public class AssignmentFile : BaseEntity
    {
        /// <summary>
        /// Оригинальное имя файла
        /// </summary>
        public string? OriginName { get; set; } = string.Empty;

        /// <summary>
        /// Имя файла присвоеное системой
        /// </summary>
        public string? SafetyName { get; set; } = string.Empty;

        /// <summary>
        /// Данные
        /// </summary>
        public byte[] Content { get; set; } = Array.Empty<byte>();

        /// <summary>
        /// Поручение к которому прикреплен файл
        /// </summary>
        public long AssignmentId { get; set; }
        public Assignment? Assignment { get; set; }

        //Навигационные свойства
        public Employee? CreatedByNavigation { get; set; }
        public Employee? UpdatedByNavigation { get; set; }
        public virtual ICollection<AssignmentHistory>? AssignmentHistoryAddedFileNavigations { get; set; }
        public virtual ICollection<AssignmentHistory>? AssignmentHistoryDeletedFileNavigations { get; set; }
    }
}