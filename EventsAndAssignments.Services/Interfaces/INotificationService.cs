using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Data.EmailTemplate;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface INotificationService
    {
        /// <summary>
        /// Добавить новое уведомление (и периодическое уведомление)
        /// </summary>
        /// <param name="template">Шаблон уведомления (может включать или не включать периодическое уведомление)</param>
        /// <param name="settings">Настройки уведомлений для пользователя</param>
        public Task<bool> AddNotificationAsync(INotificationTemplate template, NotificationSettingResponseDTO settings);

        /// <summary>
        /// Создать уведомления об истечении строка исполнения для поручений срок которых истек
        /// </summary>
        /// <returns>Количество созданных уведомлений</returns>
        Task<int> CreatePeriodicNotificationsWithExpiredExecutionDate();

        /// <summary>
        /// Обновить дату истечения срока поручения в уведомлении если она изменилась в поручении
        /// </summary>
        Task<int> UpdatePeriodicNotificationsExecutionDateAsync();

        /// <summary>
        /// Получить уведомления по электронному адресу пользователя
        /// </summary>
        /// <param name="userEmail">Электронный адрес пользователя</param>
        public Task<List<Notification>> GetNotifications(string userEmail);

        /// <summary>
        /// Выполнить отправку уведомлений по поручениям (в зависимости от изменений в поручениях)
        /// </summary>
        /// <param name="from">Поручение до изменения</param>
        /// <param name="to">Поручение после изменения</param>
        /// <param name="currentUser">Текущий пользователь</param>
        public Task<bool> SendAssignmentNotificationsAsync(Assignment from, Assignment to, Models.DTO.Common.Employee currentUser);

        /// <summary>
        /// Выполнить отправку периодических уведомлений
        /// </summary>
        /// <returns>Количество отправленных уведомлений</returns>
        public Task<int> SendScheduledNotificationsAsync();

        /// <summary>
        /// Выполнить обработку и отправить основные уведомления
        /// </summary>
        public Task<bool> ProcessNotifications();

        /// <summary>
        /// Получить настройки уведомлений для пользователя
        /// </summary>
        /// <param name="userEmail"></param>
        public Task<NotificationSettingResponseDTO> GetNotificationSettingsAsync(string userEmail);

        /// <summary>
        /// Установить настройки уведомлений для пользователя
        /// </summary>
        /// <param name="userEmail">Электронный адрес пользователя</param>
        /// <param name="request">Настройки уведомлений</param>
        public Task<bool> SetNotificationSettingsAsync(string userEmail, NotificationSettingRequestDTO request);

        /// <summary>
        /// Найти и удалить уведомления срок которых истек
        /// </summary>
        /// <param name="daysOffset">срок отсрочки удаления в днях (после истечения срока исполнения)</param>
        public Task<bool> DeleteNotificationsAsync(int daysOffset);

        /// <summary>
        /// Найти и удалить уведомления срок исполнения которых истек
        /// </summary>
        /// <param name="daysOffset">срок отсрочки удаления в днях (после истечения срока исполнения)</param>
        Task<bool> DeletePeriodicNotificationsAsync(int daysOffset);

        /// <summary>
        /// Найти и удалить уведомления об истечении строка исполнения для поручений, которые находятся в статусе ГОТОВО
        /// или в статусе КОНТРОЛЬ и назначены на контролера или в статусе В РАБОТЕ и назначены на исполнителя
        /// </summary>
        /// <returns>Количество затронутых записей </returns>
        Task<int> DeleteExpiredPeriodicNotificationsAsync();

        /// <summary>
        /// Восстановить уведомления для поручения
        /// </summary>
        /// <param name="assignment">Поручение</param>
        /// <param name="currentUser">Текущий пользователь</param>
        Task<bool> RestoreNotificationsOnAssignments(Assignment assignment, Models.DTO.Common.Employee currentUser);
    }
}