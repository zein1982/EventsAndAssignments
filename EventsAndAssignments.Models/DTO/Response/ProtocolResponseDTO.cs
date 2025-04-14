namespace EventsAndAssignments.Models.DTO.Response
{
    public class ProtocolResponseDTO : BaseDTO
    {
        /// <summary>
        /// Идентификатор протокола.
        /// </summary>
        public new long Id { get; set; }

        /// <summary>
        /// Имя протокола.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Флаг, показывающий архивирован протокол, или нет.
        /// </summary>
        public bool IsArchived { get; set; }
        public new Guid CreatedBy { get; set; }
        public string? CreatorShortName { get; set; }
    }
}