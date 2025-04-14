using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Sorts;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IProtocolService
    {
        /// <summary>
        /// Получить протоколы согласно фильтру
        /// </summary>
        /// <param name="filter">фильтр</param>
        /// <param name="currentUserEmail">Email текущего пользователя</param>
        Task<(ICollection<ProtocolResponseDTO>, int count)> GetAllAsync(RequestParams filter, string currentUserEmail);

        /// <summary>
        /// Получить данные для отчета по протоколу
        /// </summary>
        /// <param name="id">Id  протокола</param>
        Task<IReadOnlyCollection<ShortProtocolReportResponseDto>> GetShortReportData(long id);

        /// <summary>
        /// Создание протокола
        /// </summary>
        /// <param name="protocolRequest">Данные для создания протокола</param>
        /// <param name="currentUserEmail">Email текущего пользователя</param>
        Task<CreateProtocolResponseDTO> CreateAsync(CreateProtocolRequestDTO protocolRequest, string currentUserEmail);

        /// <summary>
        /// Обновить имя протокола
        /// </summary>
        /// <param name="protocolId">Id протокола</param>
        /// <param name="name">Имя протокола</param>
        /// <param name="userMail">Email пользователя</param>
        Task<CreateProtocolResponseDTO> UpdateProtocolAsync(long protocolId, string name, string userMail);

        /// <summary>
        /// Удаление протоколов
        /// </summary>
        /// <param name="id">Список id протоколов для удаления</param>
        Task<ICollection<CreateProtocolResponseDTO>> RemoveProtocolsByAdmin(IReadOnlyCollection<long> id, string description, string userMail);
    }
}