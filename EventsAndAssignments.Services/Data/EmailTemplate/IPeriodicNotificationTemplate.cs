using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public interface IPeriodicNotificationTemplate
    {
        PeriodicNotification? GetPeriodicNotification(bool isRequiredPeriodicNotifications);
    }
}