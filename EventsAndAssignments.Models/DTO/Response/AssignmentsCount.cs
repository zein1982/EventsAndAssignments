namespace EventsAndAssignments.Models.DTO.Response
{
    /// <summary>
    /// Общее количетсво поручений и количество поручений не в статусе "Готово"
    /// </summary>
    public class AssignmentsCount
    {
        /// <summary>
        /// общее количество поручений в статусе от 3 до 6
        /// контролеру счетчик будет добавляться только если поручение в 4 статусе
        /// </summary>
        public int TotalAssignments { get; set; }

        /// <summary>
        /// поручения которые просрочены
        /// </summary>
        public int UnfinishedAssignments { get; set; }
    }
}