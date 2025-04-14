namespace EventsAndAssignments.Models.DTO.Response
{
    /// <summary>
    /// Используется как ответ на создание, удаления и обновление протокола
    /// </summary>
    public class CreateProtocolResponseDTO
    {
        /// <summary>
        /// Идентификатор протокола.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Имя протокола.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Флаг, показывающий архивирован протокол, или нет.
        /// </summary>
        public bool IsArchived { get; set; }
    }
}