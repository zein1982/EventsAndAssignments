using EventsAndAssignments_DataTransfer.Interfaces;

namespace EventsAndAssignments_DataTransfer.DTO
{
    /// <summary>
    /// Событие приостановки работы сервиса
    /// </summary>
    public class ServiceSuspendedEvent : IServiceEvent
    {
        /// <summary>
        /// Создает экземпляр <see cref="ServiceSuspendedEvent"/> на основании времени и комментария события
        /// </summary>
        /// <param name="eventTime">Время события</param>
        /// <param name="message">Комментарий о событии</param>
        public ServiceSuspendedEvent(DateTime eventTime, string message)
        {
            EventDateTime = eventTime;
            EventInfo = message;
        }

        /// <inheritdoc cref="IServiceEvent.EventType"/>
        public string EventType => "Service suspended";

        /// <inheritdoc cref="IServiceEvent.EventDateTime"/>
        public DateTime EventDateTime { get; }

        /// <inheritdoc cref="IServiceEvent.EventInfo"/>
        public string EventInfo { get; }
    }
}
