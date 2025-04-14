namespace EventsAndAssignments.Models.DTO.Response
{
    public class ProtocolFolderResponse
    {
        /// <summary>
        /// Общее количество папок
        /// </summary>
        public int Count { get; set; }
        public int Page { get; set; }
        public int PageCount { get; set; }

        /// <summary>
        /// Список папок протоколов
        /// </summary>
        public List<ProtocolFolder> Items { get; set; } = new();
    }
}