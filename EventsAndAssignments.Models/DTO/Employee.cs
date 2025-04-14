namespace EventsAndAssignments.Models.DTO.Common
{
    public class Employee
    {
        /// <summary>
        /// Идентификтаор трудозанятого
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ФИО трудозанятого
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Email адрес трудозанятого
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Должность трудозанятого
        /// </summary>
        public string? Position { get; set; }

        /// <summary>
        /// Организационная единица
        /// </summary>
        public string? Department { get; set; }

        /// <summary>
        /// Организация трудозанятого
        /// </summary>
        public string? Organization { get; set; }

        /// <summary>
        /// Табельный номер трудозанятого
        /// </summary>
        public string? PersonnelNumber { get; set; }

        /// <summary>
        /// Идентификатор роли пользователя.
        /// </summary>
        public long RoleId { get; set; }
    }
}