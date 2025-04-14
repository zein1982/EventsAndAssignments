namespace EventsAndAssignments.Services.Helpers
{
    public static class NotificationsHelper
    {
        /// <summary>
        /// Метод рассчитывает дату следующей отправки обычного уведомления
        /// </summary>
        /// <param name="executionDate">Дата до которой нужно выполнить задание </param>
        /// /// <returns>Дата следующей отправки</returns>
        public static DateTime GetNextNotificationDate(DateTime executionDate)
        {
            //Рассчет следующей даты отправки уведомления по схеме 7-3-1 
            int hoursForNextNotification = 0;

            const int oneThousandYearInHours = 8766000;
            const int twoMonthsInHours = 1440;
            const int oneMonthInHours = 720;
            const int sevenDaysInHours = 168;
            const int threeDayInHours = 72;
            const int oneDayInHours = 24;

            DateTime utcNow = DateTime.UtcNow.Date;
            var executionPeriodInHours = (int)(executionDate.Date - utcNow).TotalHours;

            if (executionPeriodInHours <= 24)
            {
                return executionDate.Date; //Если осталось меньше 23 часов то отправка в день истечения срока
            }

            if (twoMonthsInHours % executionPeriodInHours is twoMonthsInHours) //проверка что на исполнение еще больше 2х месяцев
            {
                hoursForNextNotification = oneMonthInHours; //отправка раз в месяц
            }
            else if (sevenDaysInHours % executionPeriodInHours is sevenDaysInHours) //проверка что на исполнение еще больше 7 дней
            {
                //Если осталось больше 7 дней 
                int diff = executionPeriodInHours - sevenDaysInHours;
                hoursForNextNotification = sevenDaysInHours % diff is sevenDaysInHours
                    ? sevenDaysInHours //Если еще больше 14 дней
                    : diff; //Если меньше 14 то берем разницу и выравниваем отправку кратно 7
            }
            else if (threeDayInHours % executionPeriodInHours is threeDayInHours)// меньше 7 но больше 3
            {
                hoursForNextNotification = threeDayInHours; //отправка раз в 3 дня
            }
            else if (oneDayInHours % executionPeriodInHours is oneDayInHours)//меньше 3 но больше 1
            {
                hoursForNextNotification = executionPeriodInHours - oneDayInHours; //отправка в день истечения срока
            }
            else //если дата исполнения меньше одного дня то не отправляем
                 //(закидываю далеко вперед т.к отбор уведомлений по дате меньше или равно текущей дате)
            {
                hoursForNextNotification = oneThousandYearInHours;
            }

            return utcNow + TimeSpan.FromHours(hoursForNextNotification);
        }

        /// <summary>
        /// Метод рассчитывает дату следующей отправки обычного уведомления
        /// </summary>
        /// <param name="daysToNextNotify">Количество дней до следующей отправки начиная с сегодняшнего дня</param>
        /// <returns>Дата следующей отправки</returns>
        public static DateTime GetNextExpiredNotificationDate(int daysToNextNotify = 3) =>
            DateTime.UtcNow + TimeSpan.FromDays(daysToNextNotify);
    }
}