using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class NotificationGateway : INotificationGateway
    {
        readonly ApplicationDbContext _context;

        public NotificationGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<NotificationSetting?> GetNotificationSettingsAsync(Guid positionId)
        {
            return _context.NotificationSettings.AsNoTracking().Include(e => e.UserNavigation).SingleOrDefaultAsync(x => x.UserPositionId == positionId);
        }

        public Task<List<Notification>> GetNotificationAsync(string email)
        {
            return _context.Notifications.AsNoTracking().Where(x => x.Recipient == email).ToListAsync();
        }

        public Task<List<Notification>> GetNotificationForSend() =>
            _context.Notifications
                .AsNoTracking()
                .Where(x => !x.IsProcessed && x.SendDate <= DateTime.UtcNow.AddMinutes(1))
                .ToListAsync();

        public async Task<List<PeriodicNotification>> GetPeriodicNotificationsToSendAsync()
        {
            List<PeriodicNotification> periodicNotifications = await _context.PeriodicNotifications
                .Include(periodicNotification => periodicNotification.Recipient)
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.Protocol)
                .Where(e => e.SendDate.Date <= DateTime.UtcNow.Date)
                .ToListAsync();

            return periodicNotifications;
        }

        public async Task<List<PeriodicNotification>?> GetOrdinaryPeriodicNotificationsWithExpiredExecutionDateAsync()
        {
            List<PeriodicNotification> periodicNotifications = await _context.PeriodicNotifications
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.Protocol)
                .ThenInclude(protocol => protocol!.Folder)
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.ResponsibleLeader)
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.ResponsibleExecutor)
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.ResponsibleInspector)
                .Where(periodicNotification =>
                        periodicNotification.Assignment != null
                            && periodicNotification.Assignment.Protocol != null
                            && periodicNotification.Assignment.Protocol.Folder != null
                            && periodicNotification.ExecutionDate.Date <= DateTime.UtcNow.Date
                            && periodicNotification.Assignment.Removed == null
                            && periodicNotification.Assignment.Protocol.Removed == null
                            && periodicNotification.Assignment.Protocol.Folder.Removed == null
                            && periodicNotification.NotificationType == (int)PeriodicNotificationType.Ordinary //получаем только уведомления которые были до истечения срока
                    )
                .ToListAsync();

            return periodicNotifications;
        }

        public bool CreateNotification(Notification notification)
        {
            _context.Notifications.Add(notification);

            int numberUpdatedEntries = _context.SaveChanges();

            return numberUpdatedEntries > 0;
        }

        public bool CreatePeriodicNotification(PeriodicNotification notification)
        {
            _context.PeriodicNotifications.Add(notification);

            int numberUpdatedEntries = _context.SaveChanges();

            return numberUpdatedEntries > 0;
        }

        public async Task<bool> CreateOrUpdateNotificationSettingsAsync(NotificationSetting newSetting)
        {
            NotificationSetting? setting = await _context.NotificationSettings
                .SingleOrDefaultAsync(x => x.UserPositionId == newSetting.UserPositionId);

            if (setting is null)
            {
                NotificationSetting settingsToAdd=new()
                {
                    IsNew = true,
                    IsWeekly = true,
                    IsStatusChange = true,
                    UserPositionId = newSetting.UserPositionId
                };

                _context.NotificationSettings.Add(settingsToAdd);
            }
            else
            {
                setting.IsNew = newSetting.IsNew;
                setting.IsWeekly = newSetting.IsWeekly;
                setting.IsStatusChange = newSetting.IsStatusChange;
            }

            int result = await _context.SaveChangesAsync();

            return result == 1;
        }

        public async Task<bool> UpdatePeriodicNotificationSendDateAsync(long periodicNotificationId, DateTime newSendDate)
        {
            PeriodicNotification? notification = await _context.PeriodicNotifications
                .FindAsync(periodicNotificationId);

            if (notification is not null)
            {
                notification.SendDate = newSendDate; //обновляю дату отправки
            }

            bool numberUpdatedEntries = await _context.SaveChangesAsync() > 0;

            return numberUpdatedEntries;
        }

        public async Task<int> UpdatePeriodicNotificationsExecutionDateAsync()
        {
            List<PeriodicNotification>? notifications = await _context.PeriodicNotifications
                .Include(notification => notification.Assignment)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                //Если дата исполнения изменилась
                switch (notification.ResponsibleType)
                {
                    case (int)ResponsibleType.Executor:
                        if (notification.Assignment?.ExecutorExecutionDate != notification.ExecutionDate
                            && notification.Assignment?.ExecutorExecutionDate is not null)
                        {
                            notification.ExecutionDate = notification.Assignment.ExecutorExecutionDate.Value;
                        }

                        break;

                    case (int)ResponsibleType.Leader:
                        if (notification.Assignment?.LeaderExecutionDate != notification.ExecutionDate
                            && notification.Assignment?.LeaderExecutionDate is not null)
                        {
                            notification.ExecutionDate = notification.Assignment.LeaderExecutionDate.Value;
                        }

                        break;

                    case (int)ResponsibleType.Inspector:
                        if (notification.Assignment?.InspectorCheckDate != notification.ExecutionDate
                            && notification.Assignment?.InspectorCheckDate is not null)
                        {
                            notification.ExecutionDate = notification.Assignment.InspectorCheckDate.Value;
                        }

                        break;
                }
            }

            int numberUpdatedEntries = await _context.SaveChangesAsync();

            return numberUpdatedEntries;
        }

        public async Task<bool> SetNotificationProcessedAsync(long id)
        {
            Notification item= await _context.Notifications.SingleAsync(x => x.Id == id);

            item.IsProcessed = true;

            return await _context.SaveChangesAsync() == 1;
        }

        public async Task<bool> DeleteNotificationsAsync(int daysOffset)
        {
            int result = await _context.Notifications
                .Where(x => x.IsProcessed && x.SendDate.Date <= DateTime.UtcNow.Date)
                .ExecuteDeleteAsync();

            return result >= 0;
        }

        public async Task<bool> DeletePeriodicNotificationsAsync(int daysOffset)
        {
            //Получаем уведомления у которых истек срок исполнения
            List<PeriodicNotification> notificationsWithExpiredExecutionDate = await _context.PeriodicNotifications
                .Include(e => e.Assignment)
                .ThenInclude(e => e!.Protocol)
                .ThenInclude(e => e!.Folder)
                .Where(e => (e.ExecutionDate.Date <= DateTime.UtcNow.Date //удаляем только обычные периодические уведомления(если меняешь это поле проверь и создание просроченных уведомлений) 
                    && e.NotificationType == (int)PeriodicNotificationType.Ordinary) //Уведомления об истечении срока удалим отдельно
                    || (e.Assignment != null
                        && (e.Assignment.StatusId == (int)Status.Done
                            || e.Assignment.Removed != null
                            || (e.Assignment.Protocol != null
                                && e.Assignment.Protocol.Removed != null)
                            || (e.Assignment.Protocol.Folder! != null
                                && e.Assignment.Protocol.Folder.Removed != null))))
            .ToListAsync();

            _context.PeriodicNotifications.RemoveRange(notificationsWithExpiredExecutionDate);
            int result = await _context.SaveChangesAsync();

            return result >= 0;
        }

        public async Task<int> DeletePeriodicNotificationsByParamsAsync(Guid recipientId,
            long assignmentId, ResponsibleType responsibleType)
        {
            PeriodicNotification? notification = await _context.PeriodicNotifications
                .FirstOrDefaultAsync(e =>
                    e.RecipientPositionId == recipientId
                        && e.AssignmentId == assignmentId
                        && (ResponsibleType)e.ResponsibleType == responsibleType
                );

            if (notification is null)
            {
                return 0;
            }

            _context.Remove(notification);

            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteExpiredPeriodicNotificationsAsync()
        {
            //Получаем уведомления у которых статус был переведен в исполнено
            List<PeriodicNotification> notificationsInStatusDone = await _context.PeriodicNotifications
                .Include(periodicNotification => periodicNotification.Assignment)
                .ThenInclude(assignment => assignment!.Protocol)
                .ThenInclude(protocol => protocol!.Folder)
                .Where(periodicNotification => periodicNotification.NotificationType == (int)PeriodicNotificationType.AfterDeadline
                    && periodicNotification.Assignment != null
                    && (periodicNotification.Assignment.StatusId == (int)Status.Done
                        || periodicNotification.Assignment.Removed != null
                        || (periodicNotification.Assignment.Protocol != null
                            && periodicNotification.Assignment.Protocol.Removed != null)
                        || (periodicNotification.Assignment.Protocol != null
                            && periodicNotification.Assignment.Protocol.Folder.Removed != null)))
                .ToListAsync();

            //Удаляю те уведомления у которых поручения к которым они привязаны в статусе ГОТОВО
            _context.PeriodicNotifications.RemoveRange(notificationsInStatusDone);

            //Получаю уведомления на контролера по поручениям НЕ в статусе контроль и
            //на исполнителя по поручениям в НЕ в статусе в Работе
            List<PeriodicNotification> notificationsInStatusMonitoringAndWork = await _context.PeriodicNotifications
                    .Include(periodicNotification => periodicNotification.Assignment)
                    .Where(periodicNotification =>
                        periodicNotification.NotificationType == (int)PeriodicNotificationType.AfterDeadline
                            && periodicNotification.Assignment != null
                            && ((periodicNotification.ResponsibleType == (int)ResponsibleType.Inspector
                                && periodicNotification.Assignment.StatusId != (long)Status.Monitoring)
                                ||
                                (periodicNotification.ResponsibleType == (int)ResponsibleType.Executor
                                    && periodicNotification.Assignment.StatusId != (long)Status.InWork))
                    ).ToListAsync();

            //Удаляю те уведомления у которых поручения НЕ в статусе КОНТРОЛЬ и они на Контролера,
            //и НЕ в статусе В РАБОТЕ и они на исполнителя
            _context.PeriodicNotifications.RemoveRange(notificationsInStatusMonitoringAndWork);

            int result = await _context.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteExpiredPeriodicNotificationsForResponsibleExecutorsAsync()
        {
            //Получаю уведомления на исполнителя по поручениям в НЕ в статусе в Работе
            List<PeriodicNotification> notificationsInStatusWork = await _context.PeriodicNotifications
                    .Include(periodicNotification => periodicNotification.Assignment)
                    .Where(periodicNotification =>
                        periodicNotification.Assignment != null
                            && periodicNotification.ResponsibleType == (int)ResponsibleType.Executor
                            && periodicNotification.Assignment.StatusId != (long)Status.InWork)
                    .ToListAsync();

            //Удаляю те уведомления у которых поручения НЕ в статусе КОНТРОЛЬ и они на Контролера,
            //и НЕ в статусе В РАБОТЕ и они на исполнителя
            _context.PeriodicNotifications.RemoveRange(notificationsInStatusWork);

            int result = await _context.SaveChangesAsync();

            return result;
        }

        public async Task<int> DeleteExpiredPeriodicNotificationsForResponsibleInspectorsAsync()
        {
            //Получаю уведомления на контролера по поручениям НЕ в статусе контроль
            List<PeriodicNotification> notificationsInStatusMonitoring = await _context.PeriodicNotifications
                .Include(periodicNotification => periodicNotification.Assignment)
                .Where(periodicNotification =>
                    periodicNotification.Assignment != null
                        && periodicNotification.ResponsibleType == (int)ResponsibleType.Inspector
                        && periodicNotification.Assignment.StatusId != (long)Status.Monitoring)
                .ToListAsync();

            //Удаляю те уведомления у которых поручения НЕ в статусе КОНТРОЛЬ и они на Контролера,
            //и НЕ в статусе В РАБОТЕ и они на исполнителя
            _context.PeriodicNotifications.RemoveRange(notificationsInStatusMonitoring);

            int result = await _context.SaveChangesAsync();

            return result;
        }
    }
}