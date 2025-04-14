using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    class AssignmentInspectorPeriodicTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public AssignmentInspectorPeriodicTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public PeriodicNotification? GetPeriodicNotification(bool isRequiredPeriodicNotifications) =>
            isRequiredPeriodicNotifications
                ? new PeriodicNotification
                {
                    Subject = TemplateUtils.GetNotificationSubject(_assignment, "Напоминаем."),
                    Message = TemplateUtils.GetHtmlFormattedNotificationBody(
                        _assignment, "Контролера", _assignment.InspectorCheckDate, _frontUrl, "Напоминаем, что"),
                    SendDate = NotificationsHelper
                        .GetNextNotificationDate(_assignment.InspectorCheckDate!.Value),
                    ExecutionDate = _assignment.InspectorCheckDate.Value,
                    RecipientPositionId = _assignment.ResponsibleInspector!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.Ordinary,
                    ResponsibleType = (int)ResponsibleType.Inspector
                }
                : null;
    }
}