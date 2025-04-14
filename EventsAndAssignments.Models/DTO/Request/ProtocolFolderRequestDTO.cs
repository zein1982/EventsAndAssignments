namespace EventsAndAssignments.Models.DTO.Request
{
    /// <summary>
    /// Папка протокола
    /// </summary>
    public class ProtocolFolderRequestDTO : BaseDTO
    {
        /// <summary>
        /// Имя папки
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Принадлежащие папки протоколы
        /// </summary>
        public List<ProtocolDTO> Protocols { get; set; } = new();

        /// <summary>
        /// Признак архивной сущности (доступной только для чтения)
        /// </summary>
        public bool IsArchived { get; set; }
    }
}