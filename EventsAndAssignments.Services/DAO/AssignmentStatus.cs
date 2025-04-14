namespace EventsAndAssignments.Services.DAO
{
    public class AssignmentStatus
    {
        /// <summary>
        /// Идентификатор статуса
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Наименование статуса
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Код статуса
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// В зависимости от того, есть ли у поручения контролер, количество статусов поручения
        /// может изменятся. Если контролер есть у поручения может быть 2 дополнительных статуса
        /// (контроль и проверено). Это свойство отражает наличие этих 2 статусов.
        /// </summary>
        public bool IsInShortLine { get; set; } = true;

        //Навигационные свойства

        public ICollection<Assignment>? Assignments { get; set; }
        public ICollection<AssignmentHistory>? AssignmentHistoryFromStatus { get; set; }
        public ICollection<AssignmentHistory>? AssignmentHistoryToStatus { get; set; }
    }
}