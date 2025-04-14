namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Общее количетсво поручений и количество поручений не в статусе "Готово"
    /// </summary>
    class AssignmentsCount
    {
        /// <summary>
        /// Общее количество поручения трудозанятого
        /// </summary>
        public int TotalAssignments { get; set; }

        /// <summary>
        /// Количество поручений трудозанятого не в статусе "Готово"
        /// </summary>
        public int UnfinishedAssignments { get; set; }
    }
}
