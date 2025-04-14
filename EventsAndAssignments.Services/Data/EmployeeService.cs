using EventsAndAssignments.Models.DTO;
using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IEmployeeGateway _gateway;
        private readonly IMapper _mapper;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeGateway gateway, IMapper mapper)
        {
            _logger = logger;
            _gateway = gateway;
            _mapper = mapper;
        }

        public Employee GetEmployeeByEmail(string userEmail)
        {
            DAO.Employee result = _gateway.GetEmployeeByPredicate(e => e.Email!.Contains(userEmail))!;
            return _mapper.Map<Employee>(result);
        }

        public async Task<EmployeeWithAllPositionsDto?> GetEmployeeWithAllPositionsByEmail(string userEmail)
        {
            //Получаю текущего пользователя
            DAO.Employee? employee = _gateway.GetEmployeeByPredicate(e => e.Email!.Contains(userEmail));

            if (employee is null)
            {
                return null;
            }

            //Получаю Dto
            EmployeeWithAllPositionsDto employeeResponse = _mapper.Map<EmployeeWithAllPositionsDto>(employee);
            //Получаю все должности
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles = await _gateway
                .GetAllEmployeePositionsByPositionId(employeeResponse.Id);
            //Добавляю должности в Dto
            employeeResponse.AllEmployeePositionsIds = currentEmployeeAllPositionsWithRoles.Keys.ToList();

            return employeeResponse;
        }

        public IReadOnlyCollection<Employee> GetEmployeesByName(string name, int count)
        {
            IReadOnlyCollection<DAO.Employee> result = _gateway.GetEmployees(name, count);
            return _mapper.Map<IReadOnlyCollection<Employee>>(result);
        }

        public Employee? GetEmployeeById(Guid id)
        {
            DAO.Employee? result = _gateway.GetEmployeeByPredicate(e => e.PositionId == id);

            return _mapper.Map<Employee>(result!);
        }

        public Task<byte[]?> GetEmployeePhotoById(Guid id, bool isSmall)
        {
            return _gateway.GetEmployeePhotoById(id, isSmall);
        }

        public async Task<Employee> SetEmployeeRole(Guid employeeId, long roleId)
        {
            return _mapper.Map<Employee>(await _gateway.SetEmployeeRole(employeeId, roleId));
        }

        public Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByPositionId(Guid positionId) =>
            _gateway.GetAllEmployeePositionsByPositionId(positionId);

        public Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByEmployeeId(Guid employeeId) =>
            _gateway.GetAllEmployeePositionsByEmployeeId(employeeId);
    }
}