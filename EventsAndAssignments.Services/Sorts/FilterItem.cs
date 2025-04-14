namespace EventsAndAssignments.Services.Sorts
{
    public class FilterItem
    {
        /// <summary>
        /// Значение элемента фильтра
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Пометка выбран элемент фильтра или нет
        /// </summary>
        public bool Selected { get; set; }
        public string Label { get; set; } = string.Empty;
    }
}