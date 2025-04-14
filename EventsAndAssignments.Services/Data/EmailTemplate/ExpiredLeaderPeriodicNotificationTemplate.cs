using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public class ExpiredLeaderPeriodicNotificationTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public ExpiredLeaderPeriodicNotificationTemplate(Assignment assignment, string frontUrl)
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
                        _assignment, "Ответственного руководителя", _assignment.LeaderExecutionDate, _frontUrl),
                    SendDate = NotificationsHelper
                        .GetNextExpiredNotificationDate(4),
                    ExecutionDate = _assignment.LeaderExecutionDate!.Value,
                    RecipientPositionId = _assignment.ResponsibleLeader!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.AfterDeadline,
                    ResponsibleType = (int)ResponsibleType.Leader
                }
                : null;
    }
}