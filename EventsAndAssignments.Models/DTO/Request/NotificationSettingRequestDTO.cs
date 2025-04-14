using System.ComponentModel;

namespace EventsAndAssignments.Models.DTO.Request
{
    /// <summary>
    /// Настройки уведомления пользователя
    /// </summary>
    public class NotificationSettingRequestDTO
    {
        public bool IsNew { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsStatusChange { get; set; }
    }
}