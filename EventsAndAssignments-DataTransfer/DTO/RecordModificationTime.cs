namespace EventsAndAssignments_DataTransfer.DTO
{
    /// <summary>
    /// Идентификатор записи и время ее последнего изменения
    /// </summary>
    public class RecordModificationTime
    {
        // UNDONE !! Проверить, сравнение дат работает корректно с
        // учетом типов временных штампов и БД-приемнике и БД-источнике

        /// <inheritdoc cref="RecordModificationTime"/>
        public RecordModificationTime(Guid recordId, DateTime lastModificationDate)
        {
            RecordId = recordId;
            LastModificationDate = lastModificationDate;
        }

        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>
        /// Время последнего изменения информации записи
        /// </summary>
        public DateTime LastModificationDate { get; set; }
    }
}
