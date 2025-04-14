namespace EventsAndAssignments_DataTransfer.Interfaces
{
    /// <summary>
    /// Событие сервиса передачи данных
    /// </summary>
    public interface IServiceEvent
    {
        /// <summary>
        /// Тип события
        /// </summary>
        public string EventType { get; }

        /// <summary>
        /// Время события
        /// </summary>
        public DateTime EventDateTime { get; }

        /// <summary>
        /// Сведения о событии
        /// </summary>
        public string EventInfo { get; }
    }
}
