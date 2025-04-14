using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    class RemoveProtocolNotificationTemplate : INotificationTemplate
    {
        readonly ICollection<long> _foldersIds;
        readonly string _description;

        public RemoveProtocolNotificationTemplate(ICollection<long> ids, string description)
        {
            _foldersIds = ids;
            _description = description;
        }

        public Notification GetNotification(bool isRequiredPeriodicNotifications)
        {
            Notification ret = new()
            {
                //TODO поменять на хелп деск
                Recipient= "Pavel.Vilkov@evraz.com",
                SendDate= DateTime.UtcNow,
                IsProcessed = false,
                Body = GetNotificationText(),
                Title = "Запрос на удаление папок"
            };

            return ret;
        }

        public string GetNotificationText()
        {
            string ids = string.Empty;
            foreach (var id in _foldersIds)
            {
                ids += id + ", ";
            }

            string template=$"Добрый день! Прошу удалить протоколы со следующими идентификаторами {ids}" +"по причине " + _description;

            return template;
        }
    }
}