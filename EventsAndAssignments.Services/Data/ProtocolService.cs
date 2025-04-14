using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using MapsterMapper;

namespace EventsAndAssignments.Services.Data
{
    public class ProtocolService : IProtocolService
    {
        private readonly IProtocolGateway _repository;
        private readonly IProtocolFoldersGateway _protocolFolderGateway;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;

        public ProtocolService(
            IProtocolGateway repository,
            IProtocolFoldersGateway protocolFolderGateway,
            IMapper mapper,
            IEmployeeService employeeService
            )
        {
            _repository = repository;
            _protocolFolderGateway = protocolFolderGateway;
            _employeeService = employeeService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<ShortProtocolReportResponseDto>> GetShortReportData(long id)
        {
            IReadOnlyCollection <ShortProtocolReportResponseDto> response = _mapper
                .Map<IReadOnlyCollection<ShortProtocolReportResponseDto>>(await _repository.GetShortReportData(id));

            return response;
        }

        public async Task<(ICollection<ProtocolResponseDTO>, int count)> GetAllAsync(RequestParams filter, string email)
        {
            //Получаю текущего пользователя
            Models.DTO.Common.Employee employee = _employeeService.GetEmployeeByEmail(email)
                ?? throw new EntityNotFoundException();

            //Заполняю свойства запроса данными о роли и доступах текущего пользователя
            filter.RoleId = employee!.RoleId;
            filter.PositionId = employee.Id;

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee.Id);

            (ICollection<Protocol> items, int count) response =
                _repository.GetAll(filter, currentEmployeeAllPositionsWithRoles);
            //Получаю список доступных протоколов для просмотра
            //(ICollection<ProtocolResponseDTO>, int count) response = _mapper
            //    .Map<ICollection<ProtocolResponseDTO>>(_repository.GetAll(filter, currentEmployeeAllPositionsWithRoles).items);

            return (_mapper.Map<ICollection<ProtocolResponseDTO>>(response.items), response.count);
        }

        public async Task<CreateProtocolResponseDTO> CreateAsync(
            CreateProtocolRequestDTO protocolRequest,
            string currentUserEmail)
        {
            //Если при создании протокола не был назначен администратор
            //то создаем протокол от имени текущего пользователя
            if (protocolRequest.CreatedBy == Guid.Empty)
            {
                Models.DTO.Common.Employee currentEmployee = _employeeService.GetEmployeeByEmail(currentUserEmail) ??
                    throw new EntityNotFoundException();
                protocolRequest.CreatedBy = currentEmployee.Id;
            }

            Protocol newProtocol = _mapper.Map<Protocol>(protocolRequest);

            newProtocol.Name = await CreateProtocolName(newProtocol.FolderId, newProtocol.Created);
            newProtocol.UpdatedBy = protocolRequest.CreatedBy;

            Protocol created = await _repository.CreateAsync(newProtocol);
            CreateProtocolResponseDTO response = _mapper.Map<CreateProtocolResponseDTO>(created);

            return response;
        }

        public async Task<CreateProtocolResponseDTO> UpdateProtocolAsync(
            long protocolId,
            string name,
            string userMail)
        {
            Models.DTO.Common.Employee? currentEmployee =
                _employeeService.GetEmployeeByEmail(userMail) ?? throw new EntityNotFoundException(userMail);

            Protocol updated = await _repository.UpdateProtocolAsync(protocolId, name, currentEmployee.Id);
            CreateProtocolResponseDTO response = _mapper.Map<CreateProtocolResponseDTO>(updated);

            return response;
        }

        public async Task<ICollection<CreateProtocolResponseDTO>> RemoveProtocolsByAdmin(
            IReadOnlyCollection<long> id,
            string description,
            string userMail)
        {
            ICollection<CreateProtocolResponseDTO> protocols =
                _mapper.Map<ICollection<CreateProtocolResponseDTO>>(await _repository.ArchiveProtocol(id));

            return protocols;
        }

        private async Task<string> CreateProtocolName(long folderId, DateTime creationDate)
        {
            int number = await _repository.GetProtocolCountInFolder(folderId) + 1;
            string name = await _protocolFolderGateway.GetFolderName(folderId);
            return $"Протокол \u2116{number} от {creationDate:dd-MM-yyyy} ({name})";
        }
    }
}