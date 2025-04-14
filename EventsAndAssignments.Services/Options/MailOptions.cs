namespace EventsAndAssignments.Services.Options
{
    /// <summary>
    /// Настройки для отправки почты
    /// </summary>
    public class MailOptions
    {
        public const string Mail = "MAIL";

        /// <summary>
        /// Адрес почтового сервера
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// Порт через который ведется работа
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Имя пользователя под которым система отправляет электронную почту
        /// </summary>
        public string? User { get; set; } = string.Empty;

        /// <summary>
        /// Пароль пользователя под которым система отправляет электронную почту
        /// </summary>
        public string Pass { get; set; } = string.Empty;
    }
}