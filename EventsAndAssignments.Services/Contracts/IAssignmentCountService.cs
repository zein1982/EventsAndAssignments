using EventsAndAssignments.Models.DTO.Response;

namespace EventsAndAssignments.Services.Contracts
{
    /// <summary>
    /// Отдает сведения о количестве мероприятий трудозанятого
    /// </summary>
    public interface IAssignmentCountService
    {
        /// <summary>
        /// Возвращает общее количество поручений и количетво поручений не в статусе "Готово"
        /// </summary>
        public Task<AssignmentsCount> GetAssignmentsCount(string employeeEmail);
    }
}
