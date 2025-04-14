using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.DTO_GottaGetOutOfHere
{
    public class AssignmentHistoryMessage
    {
        // TODO !! Очень странное "DTO", поскольку содерджит в себе ссылку на DAO. DTO - является просто моделью данных, передаваемых внутри ИС.
        // Такая модель данных не должна содержать ссылок на DAO, Microsoft.AspNetCore.Http, и вообще иметь зависимости от методов контроллера, например.
        // В представлении некоторых людей, идеальная DTO вообще не содержит в себе ничего кроме примитивных типов, и значит и не может иметь каких-то
        // зависимостей по определению. === Требует рафакторинга. ===

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// ФИО автора изменения поручения
        /// </summary>
        public string? EmployeeFullName { get; set; }

        /// <summary>
        /// Дата изменения поручения
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Тип изменения
        /// </summary>
        public int ModificationType { get; set; }

        /// <summary>
        /// Описание изменения поручения
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Прикрепленный к поручению ответственный
        /// </summary>
        public string? AddedResponsibleExecutorFullName { get; set; }

        /// <summary>
        /// Открепленный от выполнения поручения ответственный
        /// </summary>
        public string? RemovedResponsibleExecutorFullName { get; set; }

        /// <summary>
        /// Предшедствующий статус поручения
        /// </summary>
        public AssignmentStatus? FromStatus { get; set; }

        /// <summary>
        /// Новый статус поручения
        /// </summary>
        public AssignmentStatus? ToStatus { get; set; }

        /// <summary>
        /// Удаленный файл (наименование)
        /// </summary>
        public AssignmentFile? AddedFile { get; set; }

        /// <summary>
        /// Добавленный файл (наименование)
        /// </summary>
        public AssignmentFile? RemovedFile { get; set; }
    }
}