namespace EventsAndAssignments.Services.Enums
{
    public enum AssignmentModificationTypes
    {
        /// <summary>
        /// Добавили ответственного исполнителя
        /// </summary>
        AddExecutor = 1,

        /// <summary>
        /// Удалили ответственного исполнителя
        /// </summary>
        RemoveExecutor = 2,

        /// <summary>
        /// Поменяот ответственного исполнителя
        /// </summary>
        ChangeExecutor = 3,

        /// <summary>
        /// Изменился статус поручения
        /// </summary>
        ChangeStatus = 4,

        /// <summary>
        /// Прикреплен новый файл к поручению
        /// </summary>
        AddFile = 5,

        /// <summary>
        /// Удален ранее прикрепленный файл
        /// </summary>
        RemoveFile = 6,

        /// <summary>
        /// Просмотр поручения
        /// </summary>
        OpenAssignment = 7,

        /// <summary>
        /// Добавлен ответственный руководитель
        /// </summary>
        AddLeader = 8,

        /// <summary>
        /// Удален ответственный руководитель
        /// </summary>
        RemoveLeader = 9,

        /// <summary>
        /// Изменен ответственный руководитель
        /// </summary>
        ChangeLeader = 10,

        /// <summary>
        /// Добавлен ответственный контролер
        /// </summary>
        AddInspector = 11,

        /// <summary>
        /// Удален ответственный контролер
        /// </summary>
        RemoveInspector = 12,

        /// <summary>
        /// Изменен ответственный контролер
        /// </summary>
        ChangeInspector = 13,

        /// <summary>
        /// Добавлен автор поручения
        /// </summary>
        AddAuthor = 14,

        /// <summary>
        /// Удален автор поручения
        /// </summary>
        RemoveAuthor = 15,

        /// <summary>
        /// Автор поручения изменен
        /// </summary>
        ChangeAuthor = 16
    }
}