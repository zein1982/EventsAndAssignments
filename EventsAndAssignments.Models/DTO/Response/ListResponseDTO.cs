namespace EventsAndAssignments.Models.DTO.Response
{
    /// <summary>
    /// DTO для отправки списка элементов, которые будут отображаться на странице.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListResponseDTO<T>
    {
        /// <summary>
        /// Общее количество элементов
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Номер страницы.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Количество страниц.
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// Список элементов на странице.
        /// </summary>
        public IReadOnlyCollection<T>? Items { get; set; }
    }
}