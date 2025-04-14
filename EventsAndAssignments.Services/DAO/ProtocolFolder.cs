namespace EventsAndAssignments.Services.DAO
{
    /// <summary>
    /// Папка протокола (Наименование совещания)
    /// </summary>
    public class ProtocolFolder : BaseEntity
    {
        /// <summary>
        /// Наименование протокола
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Признак архивной сущности (доступной только для чтения)
        /// </summary>
        public bool IsArchived { get; set; }

        //Навигационные свойства
        public Employee? CreatedByNavigation { get; set; }
        public Employee? UpdatedByNavigation { get; set; }
        public virtual ICollection<Employee>? AllowedEmployeesNavigation { get; set; }
        public ICollection<Protocol> Protocols { get; set; } = new List<Protocol>();
    }
}