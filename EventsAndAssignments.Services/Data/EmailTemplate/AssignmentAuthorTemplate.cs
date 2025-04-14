using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    class AssignmentAuthorTemplate : INotificationTemplate
    {
        private readonly Assignment _assignment;
        private readonly string _frontUrl;

        public AssignmentAuthorTemplate(Assignment assignment, string frontUrl)
        {
            _assignment = assignment;
            _frontUrl = frontUrl;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification notification = new()
            {
                Recipient = _assignment.Author?.Email!,
                SendDate = DateTime.UtcNow,
                IsProcessed = false,
                Body = TemplateUtils.GetHtmlFormattedNotificationBody(_assignment, "Автора", _assignment.ExecutionDate, _frontUrl),
                Title = TemplateUtils.GetNotificationSubject(_assignment)
            };

            return notification;
        }
    }
}