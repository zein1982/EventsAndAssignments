namespace EventsAndAssignments.Services.DAO
{
    public class HolidayInfo
    {
        /// <summary>
        /// Идентификатор записи
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Год
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Полная дата
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Поле отражает является ли текущая дата выходным
        /// </summary>
        public bool FreeDay { get; set; }

        /// <summary>
        /// Поле отражает является ли текущая дата праздничным выходным
        /// </summary>
        public bool Holiday { get; set; }

        /// <summary>
        /// Номер дня недели
        /// </summary>
        public short Weekday { get; set; }
    }
}