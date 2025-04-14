using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class ProtocolFoldersService : IProtocolFoldersService
    {
        private readonly IProtocolFoldersGateway _gateway;
        private readonly ILogger<ProtocolFoldersService> _logger;
        private readonly IMapper _mapper;
        private readonly IEmployeeService _employeeService;
        private readonly INotificationService _notificationService;

        public ProtocolFoldersService(
            ILogger<ProtocolFoldersService> logger,
            IProtocolFoldersGateway gateway,
            IMapper mapper,
            IEmployeeService employeeService,
            INotificationService notificationService)
        {
            _logger = logger;
            _gateway = gateway;
            _mapper = mapper;
            _employeeService = employeeService;
            _notificationService = notificationService;
        }

        public async Task<bool> CreateProtocolFolderAsync(string folderName, string userMail, ICollection<Guid> allowedEmployeesIds)
        {
            Models.DTO.Common.Employee? currentEmployee =
                _employeeService.GetEmployeeByEmail(userMail) ?? throw new EntityNotFoundException();

            bool isSuccessfully = await _gateway.CreateProtocolFolderAsync(folderName, currentEmployee.Id, currentEmployee.RoleId, allowedEmployeesIds);

            return isSuccessfully;
        }

        public async Task<bool> UpdateProtocolFolderAsync(long id, string folderName, Guid folderOwner,
            ICollection<Guid> allowedEmployeesIds, string userMail)
        {
            Models.DTO.Common.Employee currentEmployee = _employeeService.GetEmployeeByEmail(userMail) ??
                throw new EntityNotFoundException();

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(currentEmployee.Id);

            bool isSuccessfully = await _gateway.UpdateProtocolFolderAsync(
                id, folderName, folderOwner, currentEmployee.Id,
                currentEmployee.RoleId, allowedEmployeesIds, currentEmployeeAllPositionsWithRoles);

            return isSuccessfully;
        }

        public async Task<Models.DTO.Response.ProtocolFolder> GetProtocolFolderAsync(long id)
        {
            ProtocolFolder result = await _gateway.GetProtocolFolderAsync(id);
            return _mapper.Map<Models.DTO.Response.ProtocolFolder>(result);
        }

        public async Task<(IReadOnlyCollection<Models.DTO.Response.ProtocolFolder>, int count)> GetProtocolFoldersAsync(
            RequestParams filter,
            string userMail)
        {
            Models.DTO.Common.Employee employee = _employeeService.GetEmployeeByEmail(userMail) ?? throw new EntityNotFoundException();
            filter.PositionId = employee.Id;
            filter.RoleId = employee.RoleId;

            //Получаю все должности текущего пользователя
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _employeeService
                .GetAllEmployeePositionsByPositionId(employee.Id);

            (IReadOnlyCollection<ProtocolFolder> folders, int folderCount) = await _gateway
                .GetProtocolFoldersAsync(filter, currentEmployeeAllPositionsWithRoles);

            return (_mapper.Map<IReadOnlyCollection<Models.DTO.Response.ProtocolFolder>>(folders), folderCount);
        }

        public Task ArchiveProtocolFolderAsync(List<long> idList)
        {
            return _gateway.ArchiveProtocolFolderAsync(idList);
        }

        public async Task<IReadOnlyCollection<Models.DTO.Common.Employee>> GetEmployeesAllowedToFolder(long folderId)
        {
            IReadOnlyCollection<Employee> allowedEmployees =  await _gateway.GetEmployeesAllowedToFolderAsync(folderId);
            return _mapper.Map<IReadOnlyCollection<Models.DTO.Common.Employee>>(allowedEmployees);
        }

        public Task<bool> AddAllowedEmployeeAsync(long folderId, Guid employeeId, string currentEmployeeMail)
        {
            Models.DTO.Common.Employee currentEmployee = _employeeService.GetEmployeeByEmail(currentEmployeeMail) ??
                throw new EntityNotFoundException();
            return _gateway.AddAllowedEmployeeAsync(folderId, employeeId, currentEmployee.Id, currentEmployee.RoleId);
        }

        public Task<bool> RemoveAllowedEmployeeAsync(long folderId, Guid employeeId, string currentEmployeeMail)
        {
            Models.DTO.Common.Employee currentEmployee = _employeeService.GetEmployeeByEmail(currentEmployeeMail) ??
                throw new EntityNotFoundException();
            return _gateway.RemoveAllowedEmployeeAsync(folderId, employeeId, currentEmployee.Id, currentEmployee.RoleId);
        }

        public async Task<IReadOnlyCollection<Models.DTO.Response.ProtocolFolder>> RemoveProtocolFolderAsync(
            IReadOnlyCollection<long> idsList)
        {
            IReadOnlyCollection<ProtocolFolder> result = await _gateway.RemoveProtocolFolderAsync(idsList);
            return _mapper.Map<IReadOnlyCollection<Models.DTO.Response.ProtocolFolder>>(result);
        }
    }
}