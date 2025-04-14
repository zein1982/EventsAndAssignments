using System.Net.Http.Headers;

namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Настройки уведомления пользователя
    /// </summary>
    public class NotificationSetting
    {
        public long Id { get; set; }
        public Guid UserPositionId { get; set; }
        public Employee UserNavigation { get; set; } = null!;
        public bool IsNew { get; set; }
        public bool IsWeekly { get; set; }
        public bool IsStatusChange { get; set; }
    }
}