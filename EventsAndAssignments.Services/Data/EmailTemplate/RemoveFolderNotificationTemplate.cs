using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Data.EmailTemplate
{
    class RemoveFolderNotificationTemplate : INotificationTemplate
    {
        readonly ICollection<long> _foldersIds;
        readonly string _description;

        public RemoveFolderNotificationTemplate(
            ICollection<long> ids,
            string description)
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

            bool t = File.Exists(Directory.GetCurrentDirectory()+@"\Recources\removeFolder.html");
            string te = Directory.GetCurrentDirectory();
            string template = File.ReadAllText(Directory.GetCurrentDirectory() + @"\Recources\removeFolder.html");
            //string template = "<html>"
            //    + "<body>"
            //    + $"<h1>{_userMail} отправил заявку на удаление папок</h1>"
            //    + $"<p>Прошу удалить папки со следующими идентификаторами {ids} по причине {_description}.</p>"
            //    + "<p><a href=>Смотреть папки</a></p></body></html>";
            return template;
        }
    }
}