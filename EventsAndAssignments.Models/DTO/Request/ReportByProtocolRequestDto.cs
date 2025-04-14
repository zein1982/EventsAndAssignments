namespace EventsAndAssignments.API.DTO.Request
{
    public class ReportByProtocolRequestDto
    {
        /// <summary>
        /// Дата начала периода за который нужно получить отчет.
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Дата конца периода за который нужно получить отчет.
        /// </summary>
        public DateTime DateFinish { get; set; }
    }
}