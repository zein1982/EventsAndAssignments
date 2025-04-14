using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;

namespace EventsAndAssignments.Services.Contracts
{
    public interface INotificationGateway
    {
        /// <summary>
        /// Создать или обновить настройки уведомлений для текущего пользователя
        /// </summary>
        /// <param name="newSetting">настройки</param>
        Task<bool> CreateOrUpdateNotificationSettingsAsync(NotificationSetting newSetting);

        /// <summary>
        /// Получить настройки уведомлений для пользователя
        /// </summary>
        /// <param name="positionId">идентификатор пользователя</param>
        Task<NotificationSetting?> GetNotificationSettingsAsync(Guid positionId);

        /// <summary>
        /// Получить уведомления для конкретного пользователя
        /// </summary>
        /// <param name="email">адрес электронной почты пользователя</param>
        Task<List<Notification>> GetNotificationAsync(string email);

        /// <summary>
        /// Получить все активные оповещения для отправки
        /// </summary>
        Task<List<Notification>> GetNotificationForSend();

        /// <summary>
        /// Получить все активные периодические оповещения для отправки
        /// </summary>
        Task<List<PeriodicNotification>> GetPeriodicNotificationsToSendAsync();

        /// <summary>
        /// Получить все ОБЫЧНЫЕ (<see cref="PeriodicNotificationType"/>) периодические уведомления с истекшим сроком исполнения. В НЕ удаленных поручениях папках и протоколах
        /// </summary>
        Task<List<PeriodicNotification>?> GetOrdinaryPeriodicNotificationsWithExpiredExecutionDateAsync();

        /// <summary>
        /// Создать новые оповещения основное и периодическое (если есть)
        /// </summary>
        /// <param name="notification">уведомление</param>
        bool CreateNotification(Notification notification);

        /// <summary>
        /// Создать только периодическое уведомление
        /// </summary>
        /// <param name="notification">Уведомление</param>
        bool CreatePeriodicNotification(PeriodicNotification notification);

        /// <summary>
        /// Обновить дату отправки для периодического уведомления
        /// </summary>
        /// <param name="periodicNotificationId">Идентификатор периодического обновленияя</param>
        /// <param name="newSendDate">новая дата отправки</param>
        Task<bool> UpdatePeriodicNotificationSendDateAsync(long periodicNotificationId, DateTime newSendDate);

        /// <summary>
        /// Обновить дату истечения срока уведомления если она изменилась в поручении
        /// </summary>
        Task<int> UpdatePeriodicNotificationsExecutionDateAsync();

        /// <summary>
        /// Установка флага о том что уведомление было обработано и отправлено для основных типов уведомлений
        /// </summary>
        /// <param name="id">Идентификатор уведомления</param>
        Task<bool> SetNotificationProcessedAsync(long id);

        /// <summary>
        /// Найти и удалить уведомления срок исполнения которых истек
        /// </summary>
        /// <param name="daysOffset">срок отсрочки удаления в днях (после истечения срока исполнения)</param>
        Task<bool> DeleteNotificationsAsync(int daysOffset);

        /// <summary>
        /// Найти и удалить уведомления срок исполнения которых истек
        /// </summary>
        /// <param name="daysOffset">срок отсрочки удаления в днях (после истечения срока исполнения)</param>
        Task<bool> DeletePeriodicNotificationsAsync(int daysOffset);

        /// <summary>
        /// Найти и удалить уведомления об истечении строка исполнения для поручений, которые находятся в статусе ГОТОВО
        /// или в статусе КОНТРОЛЬ и назначены на контролера или в статусе В РАБОТЕ и назначены на исполнителя
        /// </summary>
        Task<int> DeleteExpiredPeriodicNotificationsAsync();

        /// <summary>
        /// Найти и удалить периодическое уведомление согласно переданным параметрам
        /// </summary>
        /// <param name="recipientId">Id получателя</param>
        /// <param name="assignmentId">Id поручения</param>
        /// <param name="responsibleType">Тип ответственного в уведомлении</param>
        Task<int> DeletePeriodicNotificationsByParamsAsync(Guid recipientId,
            long assignmentId, ResponsibleType responsibleType);

        /// <summary>
        /// Найти и удалить уведомления об истечении строка исполнения для поручений, которые находятся в статусе В РАБОТЕ
        /// и назначены на исполнителя
        /// </summary>
        Task<int> DeleteExpiredPeriodicNotificationsForResponsibleExecutorsAsync();

        /// <summary>
        /// Найти и удалить уведомления об истечении строка исполнения для поручений, которые находятся в статусе КОНТРОЛЬ
        /// и назначены на контролера
        /// </summary>
        Task<int> DeleteExpiredPeriodicNotificationsForResponsibleInspectorsAsync();
    }
}