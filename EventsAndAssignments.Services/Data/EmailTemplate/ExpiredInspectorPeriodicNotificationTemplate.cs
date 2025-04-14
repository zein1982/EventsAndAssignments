using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public class ExpiredInspectorPeriodicNotificationTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public ExpiredInspectorPeriodicNotificationTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public PeriodicNotification? GetPeriodicNotification(bool isRequiredPeriodicNotifications) =>
            isRequiredPeriodicNotifications
                ? new PeriodicNotification
                {
                    Subject = TemplateUtils.GetNotificationSubject(_assignment, "Просрочено."),
                    Message = TemplateUtils.GetHtmlFormattedExpiredNotificationBody(
                        _assignment, "Контролера", _assignment.InspectorCheckDate, _frontUrl),
                    SendDate = NotificationsHelper
                        .GetNextExpiredNotificationDate(4),
                    ExecutionDate = _assignment.InspectorCheckDate!.Value,
                    RecipientPositionId = _assignment.ResponsibleInspector!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.AfterDeadline,
                    ResponsibleType = (int)ResponsibleType.Inspector
                }
                : null;
    }
}