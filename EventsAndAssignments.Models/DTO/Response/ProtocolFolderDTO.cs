using EventsAndAssignments.Models.DTO.Common;

namespace EventsAndAssignments.Models.DTO.Response
{
    /// <summary>
    /// Папка протокола (Наименование совещания)
    /// </summary>
    public class ProtocolFolder : BaseDTO
    {
        /// <summary>
        /// Наименование папки протокола
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Индикатор, удален протокол или нет.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Фамилия и инициалы создателя папки
        /// </summary>
        public string SurnameInitials { get; set; } = string.Empty;

        /// <summary>
        /// Полное имя создателя папки
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия и инициалы пользователей с доступом к папке
        /// </summary>
        public ICollection<Employee> AllowedEmployees { get; set; } = new List<Employee>();
    }
}