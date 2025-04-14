namespace EventsAndAssignments.Models.DTO.Response
{
    public class AssignmentHistoryResponseDto
    {
        /// <summary>
        /// ФИО автора изменения поручения
        /// </summary>
        public string? EmployeeFullName { get; set; }

        /// <summary>
        /// Дата изменения поручения
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Описание изменения поручения
        /// </summary>
        public string? Description { get; set; }
    }
}