using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IReportService
    {
        /// <summary>
        /// Получение отчета по протоколам
        /// </summary>
        MemoryStream MakeReportByProtocol(List<long> ids);

        /// <summary>
        /// Получение отчета по списку поручений
        /// </summary>
        /// <param name="ids">Список идентификаторов</param>
        /// <param name="timeDifference">Разница во времени</param>
        MemoryStream MakeShortReportByAssignments(List<long> ids, int timeDifference);

        /// <summary>
        /// Получение отчета протокола по списку его поручений
        /// </summary>
        MemoryStream MakeReportByAssignments(List<Assignment> dataForReport, int timeDifference);

        /// <summary>
        /// Получить данные для отчета по протоколам
        /// </summary>
        /// <param name="id">Id протокола</param>
        IReadOnlyCollection<Assignment> GetDataForExcelProtocolReport(long id);
    }
}