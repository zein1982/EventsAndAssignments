using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAndAssignments.Services.DAO
{
    public class EmployeeDirectory
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid? PositionId { get; set; }

        /// <summary>
        /// Конкатенированные фамилия, имя, отчество трудозанятого
        /// </summary>
        public string? FullUserName { get; set; }
    }
}
