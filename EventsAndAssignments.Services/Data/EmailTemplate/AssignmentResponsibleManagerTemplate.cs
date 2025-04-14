using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Helpers;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    /// <summary>
    /// Ответственного руководителя по новому поручению
    /// </summary>
    public class AssignmentResponsibleManagerTemplate : INotificationTemplate
    {
        private readonly Assignment _assignment;
        private readonly string _frontUrl;

        public AssignmentResponsibleManagerTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification notification = new()
            {
                Recipient=_assignment.ResponsibleLeader?.Email!,
                SendDate= DateTime.UtcNow,
                IsProcessed = false,
                Body = TemplateUtils.GetHtmlFormattedNotificationBody(_assignment, "Ответственного руководителя", _assignment.LeaderExecutionDate, _frontUrl),
                Title = TemplateUtils.GetNotificationSubject(_assignment),
                PeriodicNotification = isRequiredPeriodicNotifications
                    ? new PeriodicNotification
                    {
                        Subject = TemplateUtils.GetNotificationSubject(_assignment, "Напоминаем."),
                        Message = TemplateUtils.GetHtmlFormattedNotificationBody(
                            _assignment, "Ответственного руководителя", _assignment.LeaderExecutionDate, _frontUrl, "Напоминаем, что"),
                        SendDate = NotificationsHelper
                            .GetNextNotificationDate(_assignment.LeaderExecutionDate!.Value),
                        ExecutionDate = _assignment.LeaderExecutionDate!.Value,
                        RecipientPositionId = _assignment.ResponsibleLeader!.PositionId,
                        AssignmentId = _assignment.Id,
                        NotificationType = (int)PeriodicNotificationType.Ordinary,
                        ResponsibleType = (int)ResponsibleType.Leader
                    }
                    : null
            };

            return notification;
        }
    }
}