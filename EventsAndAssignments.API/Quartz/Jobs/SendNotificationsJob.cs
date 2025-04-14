using EventsAndAssignments.Services.Interfaces;
using Quartz;

namespace EventsAndAssignments.API.Quartz.Jobs
{
    public class SendNotificationsJob : IJob
    {
        private readonly ILogger<SendNotificationsJob> _logger;
        private readonly INotificationService _notificationService;
        private readonly int _daysDelayForClearOldNotifications;

        public SendNotificationsJob(
            ILogger<SendNotificationsJob> logger,
            INotificationService notificationService)
        {
            _logger = logger;
            _notificationService = notificationService;
            _daysDelayForClearOldNotifications = 0;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            //Обновление дат истечения срока по поручению  у уведомлений если она изменилась в поручении
            //int countUpdatedNotifications = await _notificationService.UpdatePeriodicNotificationsExecutionDateAsync();
            //if (countUpdatedNotifications > 0)
            //{
            //    _logger.LogInformation(
            //        "У {count} уведомлений была изменена дата истечения срока по поручению",
            //        countUpdatedNotifications);
            //}

            //Создание новых уведомлений о просроченных поручениях (порядок не менять)
            int countCreatedNotificationsWithExpiredExecutionDate = await _notificationService
                .CreatePeriodicNotificationsWithExpiredExecutionDate();

            //Удаление уведомлений с истекшим сроком исполнения

            bool isNotificationsSuccessfullyDeleted =
                await _notificationService.DeleteNotificationsAsync(_daysDelayForClearOldNotifications);
            bool isPeriodicNotificationsSuccessfullyDeleted =
                await _notificationService.DeletePeriodicNotificationsAsync(_daysDelayForClearOldNotifications);
            int countDeletedExpiredPeriodicNotifications =
                await _notificationService.DeleteExpiredPeriodicNotificationsAsync();

            _logger.LogInformation("{NotificationCount} успешно удалено!", countDeletedExpiredPeriodicNotifications);

            if (isNotificationsSuccessfullyDeleted)
            {
                _logger.LogInformation("Уведомления с истекшим сроком исполнения успешно удалены");
            }

            if (isPeriodicNotificationsSuccessfullyDeleted)
            {
                _logger.LogInformation("Периодические уведомления с истекшим сроком исполнения успешно удалены");
            }

            _logger.LogInformation("Job with Key: [{JobKey}] started", context.JobDetail.Key);

            //Отправка периодических уведомлений
            int messageCount = await _notificationService.SendScheduledNotificationsAsync();

            _logger.LogInformation("Отправка уведомлений завершена. Отправлено {MessageCount} уведомлений.",
                messageCount);
        }
    }
}