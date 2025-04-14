using EventsAndAssignments_DataTransfer.Interfaces;

namespace EventsAndAssignments_DataTransfer.DTO
{
    /// <summary>
    /// Событие возобновления работы сервиса
    /// </summary>
    public class ServiceContinuedEvent : IServiceEvent
    {
        /// <summary>
        /// Создает экземпляр <see cref="ServiceContinuedEvent"/> на основании времени и комментария события
        /// </summary>
        /// <param name="eventTime">Время события</param>
        /// <param name="message">Комментарий о событии</param>
        public ServiceContinuedEvent(DateTime eventTime, string message)
        {
            EventDateTime = eventTime;
            EventInfo = message;
        }

        /// <inheritdoc cref="IServiceEvent.EventType"/>
        public string EventType => "Service continued";

        /// <inheritdoc cref="IServiceEvent.EventDateTime"/>
        public DateTime EventDateTime { get; }

        /// <inheritdoc cref="IServiceEvent.EventInfo"/>
        public string EventInfo { get; }
    }
}
