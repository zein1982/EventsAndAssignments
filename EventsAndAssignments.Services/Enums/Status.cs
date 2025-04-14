namespace EventsAndAssignments.Services.Enums
{
    public enum Status
    {
        /// <summary>
        /// Новое
        /// </summary>
        Created = 1,

        /// <summary>
        /// Назначено
        /// </summary>
        Assign = 2,

        /// <summary>
        /// В работе
        /// </summary>
        InWork = 3,

        /// <summary>
        /// Контроль
        /// </summary>
        Monitoring = 4,

        /// <summary>
        /// Проверенно
        /// </summary>
        Verified = 5,

        /// <summary>
        /// Исполненно
        /// </summary>
        Completed = 6,

        /// <summary>
        /// Готово
        /// </summary>
        Done = 7
    }
}