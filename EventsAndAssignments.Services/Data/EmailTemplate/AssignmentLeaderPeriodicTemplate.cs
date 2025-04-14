using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public class AssignmentLeaderPeriodicTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public AssignmentLeaderPeriodicTemplate(Assignment assignment, string frontUrl)
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
                        _assignment, "Ответственного руководителя", _assignment.LeaderExecutionDate, _frontUrl, "Напоминаем, что"),
                    SendDate = NotificationsHelper
                        .GetNextNotificationDate(_assignment.LeaderExecutionDate!.Value),
                    ExecutionDate = _assignment.LeaderExecutionDate.Value,
                    RecipientPositionId = _assignment.ResponsibleLeader!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.Ordinary,
                    ResponsibleType = (int)ResponsibleType.Leader
                }
                : null;
    }
}