using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    public class AssignmentExecutorPeriodicTemplate : IPeriodicNotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public AssignmentExecutorPeriodicTemplate(Assignment assignment, string frontUrl)
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
                        _assignment, "Ответственного исполнителя", _assignment.ExecutorExecutionDate, _frontUrl, "Напоминаем, что"),
                    SendDate = NotificationsHelper
                        .GetNextNotificationDate(_assignment.ExecutorExecutionDate!.Value),
                    ExecutionDate = _assignment.ExecutorExecutionDate.Value,
                    RecipientPositionId = _assignment.ResponsibleExecutor!.PositionId,
                    AssignmentId = _assignment.Id,
                    NotificationType = (int)PeriodicNotificationType.Ordinary,
                    ResponsibleType = (int)ResponsibleType.Executor
                }
                : null;
    }
}