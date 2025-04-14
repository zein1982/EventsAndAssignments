using EventsAndAssignments_DataTransfer.Interfaces;

namespace EventsAndAssignments_DataTransfer.DTO
{
    /// <summary>
    /// Информация о результате передачи данных между БД
    /// </summary>
    public class DataTransferResult : IServiceEvent
    {
        /// <summary>
        /// Создает экземпляр <see cref="DataTransferResult"/> на основании результата передачи данных
        /// </summary>
        /// <param name="transferTime">Время передачи данных</param>
        /// <param name="transferSuccess">Передача данных была успешна</param>
        /// <param name="numberOfNewRecords">Количество новых (переданных) записей</param>
        /// <param name="totalNumberOfRecords">Общее количество записей (после передачи)</param>
        /// <param name="message">Сообщение про результат передачи</param>
        public DataTransferResult(DateTime transferTime, bool transferSuccess,
            int numberOfNewRecords, int totalNumberOfRecords, string? message)
        {
            EventDateTime = transferTime;
            EventInfo =
                $"Transfer success: {transferSuccess}; New records: {numberOfNewRecords};"
                    + $" All records: {totalNumberOfRecords}; Message: {message};";
        }

        /// <inheritdoc cref="IServiceEvent.EventType"/>
        public string EventType => "Data tranfser completed";

        /// <inheritdoc cref="IServiceEvent.EventDateTime"/>
        public DateTime EventDateTime { get; }

        /// <inheritdoc cref="IServiceEvent.EventInfo"/>
        public string EventInfo { get; }
    }
}
