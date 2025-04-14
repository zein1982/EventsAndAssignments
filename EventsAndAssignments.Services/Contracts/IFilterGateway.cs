using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    /// <summary>
    /// Методы для получения данных в зону фильтров
    /// </summary>
    public interface IFilterGateway
    {
        /// <summary>
        /// Подучение списка отетственных руководителей в поручениях
        /// </summary>
        IReadOnlyCollection<Employee?> GetResponsibleLeaders();

        /// <summary>
        /// Получение списка ответственных исполнителей в поручениях
        /// </summary>
        IReadOnlyCollection<Employee?> GetResponsibleExecutors();

        /// <summary>
        /// Получение списка контролеров в поручениях
        /// </summary>
        IReadOnlyCollection<Employee?> GetResponsibleInspectors();

        /// <summary>
        /// Получаем список папок
        /// </summary>
        IReadOnlyCollection<ProtocolFolder> GetProtocolFolders();

        /// <summary>
        /// Получение списка администраторов и супер администраторов
        /// </summary>
        IReadOnlyCollection<Employee?> GetAdministrators();
    }
}