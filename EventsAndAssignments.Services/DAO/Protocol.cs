namespace EventsAndAssignments.Services.DAO
{
    public class Protocol : BaseEntity
    {
        /// <summary>
        /// Имя (номер) протокола
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Признак архивной сущности (доступной только для чтения)
        /// </summary>
        public bool IsArchived { get; set; }

        //Навигационные свойства

        /// <summary>
        /// Папка, которой принадлежит протокол
        /// </summary>
        public long FolderId { get; set; }
        public ProtocolFolder? Folder { get; set; }
        public Employee? CreatedByNavigation { get; set; }
        public Employee? UpdatedByNavigation { get; set; }
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
    }
}