using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    /// <summary>
    /// Ответственного контролера по новому поручению
    /// </summary>
    public class AssignmentInspectorTemplate : INotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _frontUrl;

        public AssignmentInspectorTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification notification = new()
            {
                Recipient=_assignment.ResponsibleInspector?.Email!,
                SendDate= DateTime.UtcNow,
                IsProcessed = false,
                Body = TemplateUtils.GetHtmlFormattedNotificationBody(_assignment, "Контролера", _assignment.InspectorCheckDate, _frontUrl),
                Title= TemplateUtils.GetNotificationSubject(_assignment),
                PeriodicNotification = null
                    //isRequiredPeriodicNotifications
                    //? new PeriodicNotification
                    //{
                    //    Subject = TemplateUtils.GetNotificationSubject(_assignment, "Напоминаем."),
                    //    Message = TemplateUtils.GetHtmlFormattedNotificationBody(
                    //        _assignment, "Контролера", _assignment.InspectorCheckDate, _frontUrl, "Напоминаем, что"),
                    //    SendDate = NotificationsHelper
                    //        .GetNextNotificationDate(_assignment.InspectorCheckDate!.Value),
                    //    ExecutionDate = _assignment.InspectorCheckDate.Value,
                    //    RecipientPositionId = _assignment.ResponsibleInspector!.PositionId,
                    //    AssignmentId = _assignment.Id
                    //}
                    //: null
            };

            return notification;
        }
    }
}