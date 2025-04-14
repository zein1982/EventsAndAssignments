using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    /// <summary>
    /// Изменения статуса
    /// </summary>
    public class StatusChangeTemplate : INotificationTemplate
    {
        readonly Assignment _assignment;
        readonly string _recipientEmail;
        private readonly string _frontUrl;

        public StatusChangeTemplate(Assignment assignment, string recipientEmail, string frontUrl)
        {
            _assignment = assignment;
            _recipientEmail = recipientEmail;
            _frontUrl = frontUrl;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification ret = new()
            {
                Recipient = _recipientEmail,
                SendDate = DateTime.UtcNow,
                IsProcessed = false,
                Body = TemplateUtils.GetHtmlFormattedStatusNotificationBody(_assignment, _frontUrl),
                Title = TemplateUtils.GetNotificationSubject(_assignment, "Статус изменился."),
            };

            return ret;
        }
    }
}