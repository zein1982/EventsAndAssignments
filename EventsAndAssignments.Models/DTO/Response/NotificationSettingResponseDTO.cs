using System.Net.Http.Headers;

namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Настройки уведомления пользователя
    /// </summary>
    public class NotificationSettingResponseDTO
    {
        public string UserEmail { get; set; }
        public string NewTitle { get; set; }
        public bool IsNew { get; set; }
        public string WeeklyTitle { get; set; }
        public bool IsWeekly { get; set; }
        public string StatusChangeTitle { get; set; }
        public bool IsStatusChange { get; set; }
    }
}