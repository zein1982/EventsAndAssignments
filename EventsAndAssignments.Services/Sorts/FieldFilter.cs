using EventsAndAssignments.Services.Enums;

namespace EventsAndAssignments.Services.Sorts
{
    /// <summary>
    /// Фильтр
    /// </summary>
    public class FieldFilter
    {
        /// <summary>
        /// Название фильтра
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Тип фильра.
        /// </summary>
        public FilterEnum FilterType { get; set; }
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Список значений фильтра
        /// </summary>
        public List<FilterItem> Items { get; set; } = new();
    }
}