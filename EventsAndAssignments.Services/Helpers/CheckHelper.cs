namespace EventsAndAssignments.Services.Helpers
{
    /// <summary>
    /// Вспомогательный класс для проверки добавления, изменения и удаления сущностей из объекта
    /// </summary>
    public static class CheckHelper
    {
        /// <summary>
        /// Проверка добавления записи
        /// </summary>
        /// <typeparam name="T">тип параметра записи</typeparam>
        /// <param name="from">Значение параметра начальное</param>
        /// <param name="to">Значение параметра текущее</param>
        public static bool IsAdded<T>(T from, T to) =>
            EqualityComparer<T>.Default.Equals(from, default)
                && !EqualityComparer<T>.Default.Equals(to, default);

        /// <summary>
        /// Проверка изменения записи
        /// </summary>
        /// <typeparam name="T">тип параметра записи</typeparam>
        /// <param name="from">Значение параметра начальное</param>
        /// <param name="to">Значение параметра текущее</param>
        public static bool IsChanged<T>(T from, T to) =>
            !EqualityComparer<T>.Default.Equals(from, default)
                && !EqualityComparer<T>.Default.Equals(to, default)
                && !EqualityComparer<T>.Default.Equals(from, to);

        /// <summary>
        /// Проверка удаления записи
        /// </summary>
        /// <typeparam name="T">тип параметра записи</typeparam>
        /// <param name="from">Значение параметра начальное</param>
        /// <param name="to">Значение параметра текущее</param>
        public static bool IsRemoved<T>(T from, T to) =>
            !EqualityComparer<T>.Default.Equals(from, default)
                && EqualityComparer<T>.Default.Equals(to, default);
    }
}