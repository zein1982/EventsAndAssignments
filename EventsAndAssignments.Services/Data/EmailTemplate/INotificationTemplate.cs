using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public interface INotificationTemplate
    {
        Notification GetNotification(bool isRequiredPeriodicNotifications);
    }
}