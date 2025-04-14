using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    /// <summary>
    /// Ответственного исполнителя по новому поручению
    /// </summary>
    public class AssignmentResponsibleExecutorTemplate : INotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public AssignmentResponsibleExecutorTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification notification = new()
            {
                Recipient=_assignment.ResponsibleExecutor!.Email!,
                SendDate= DateTime.UtcNow,
                IsProcessed = false,
                Body = TemplateUtils.GetHtmlFormattedNotificationBody(_assignment, "Ответственного исполнителя", _assignment.ExecutorExecutionDate, _frontUrl),
                Title = TemplateUtils.GetNotificationSubject(_assignment),
                PeriodicNotification = isRequiredPeriodicNotifications
                    ? new PeriodicNotification
                    {
                        Subject = TemplateUtils.GetNotificationSubject(_assignment, "Напоминаем."),
                        Message = TemplateUtils.GetHtmlFormattedNotificationBody(
                            _assignment, "Ответственного исполнителя", _assignment.ExecutorExecutionDate, _frontUrl, "Напоминаем, что"),
                        SendDate = NotificationsHelper
                            .GetNextNotificationDate(_assignment.ExecutorExecutionDate!.Value),
                        ExecutionDate = _assignment.ExecutorExecutionDate!.Value,
                        RecipientPositionId = _assignment.ResponsibleExecutor!.PositionId,
                        AssignmentId = _assignment.Id,
                        NotificationType = (int)PeriodicNotificationType.Ordinary,
                        ResponsibleType = (int)ResponsibleType.Executor
                    }
                    : null
            };

            return notification;
        }
    }
}