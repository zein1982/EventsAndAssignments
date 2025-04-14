namespace EventsAndAssignments.Services.Sorts
{
    /// <summary>
    /// Сортировка
    /// </summary>
    public class FieldSort
    {
        /// <summary>
        /// Имя столбца для сортировки
        /// </summary
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// критери того будем мы сортировать по данному столбцу или нет
        /// </summary>
        public bool Selected { get; set; }

        /// <summary>
        /// Направление сортировки
        /// descending-по убыванию
        /// ascending-по возрастанию
        /// </summary>
        public string SortDirection { get; set; } = "ascending";
    }
}