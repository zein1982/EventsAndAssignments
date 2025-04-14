using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public class ExpiredExecutorPeriodicNotificationTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public ExpiredExecutorPeriodicNotificationTemplate(Assignment assignment, string frontUrl)
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
                        _assignment, "Ответственного исполнителя", _assignment.ExecutorExecutionDate, _frontUrl),
                    SendDate = NotificationsHelper
                        .GetNextExpiredNotificationDate(4),
                    ExecutionDate = _assignment.ExecutorExecutionDate!.Value,
                    RecipientPositionId = _assignment.ResponsibleExecutor!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.AfterDeadline,
                    ResponsibleType = (int)ResponsibleType.Executor
                }
                : null;
    }
}