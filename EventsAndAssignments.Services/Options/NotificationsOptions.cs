namespace EventsAndAssignments.Services.Options
{
    /// <summary>
    /// Настройки уведомлений
    /// </summary>
    public class NotificationsOptions
    {
        public const string Notifications = "Notifications";

        /// <summary>
        /// Базовый адрес приложения (используется для отправки ссылки клиентам)
        /// </summary>
        public string FrontUrl { get; set; } = string.Empty;
    }
}