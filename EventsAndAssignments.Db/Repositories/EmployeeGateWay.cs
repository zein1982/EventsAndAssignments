using System.Linq.Expressions;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class EmployeeGateway : IEmployeeGateway
    {
        private readonly ApplicationDbContext _context;

        public EmployeeGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<Employee> GetEmployees(string? userName = null, int count = 80)
        {
            string pattern = "%" + userName + "%";
            List<Employee> employees = _context.Employees
                .Where(x => EF.Functions.Like(x.LastName + " " + x.FirstName + " " + x.MiddleName, pattern))
                .GroupBy(employee => employee.EmployeeId) //беру сотрудников с max полем occupation
                .Select(grouping => grouping
                    .OrderByDescending(e => e.IsActive)
                    .ThenByDescending(e => e.Occupation)
                    .First())
                .Take(count)
                .ToList();

            return employees;
        }

        public Employee? GetEmployeeByPredicate(Expression<Func<Employee, bool>> predicate)
        {
            return _context.Employees
                .AsNoTracking()
                .Where(predicate)
                .GroupBy(predicate)// получаю сотрудника с максимальным значением поля occupation
                .Select(employee => employee
                    .OrderByDescending(e => e.IsActive)
                    .ThenByDescending(e => e.Occupation)
                    .FirstOrDefault())
                .FirstOrDefault();
        }

        public Task<byte[]?> GetEmployeePhotoById(Guid id, bool isSmall)
        {
            return _context.Employees
                .AsNoTracking()
                .Where(e => e.PositionId == id)
                .Select(e => isSmall ? e.PhotoS : e.Photo)
                .FirstOrDefaultAsync();
        }

        public async Task<Employee> SetEmployeeRole(Guid id, long roleId)
        {
            Employee updated = await _context.Employees.SingleAsync(x => x.PositionId == id);
            updated.RoleId = roleId;

            await _context.SaveChangesAsync();

            return updated;
        }

        public async Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByPositionId(Guid positionId)
        {
            Guid employeeId = await _context.Employees
                .Where(employee => employee.PositionId == positionId)
                .Select(employee => employee.EmployeeId).FirstAsync();

            IReadOnlyDictionary<Guid, long?> employeePositionsIds = await GetAllEmployeePositionsByEmployeeId(employeeId);

            return employeePositionsIds;
        }

        public async Task<IReadOnlyDictionary<Guid, long?>> GetAllEmployeePositionsByEmployeeId(Guid employeeId)
        {
            IReadOnlyDictionary<Guid, long?> employeePositionsIds = await _context.Employees
                .Where(employee => employee.EmployeeId == employeeId)
                .Select(employee => new {employee.PositionId, employee.RoleId})
                .ToDictionaryAsync(employee => employee.PositionId,
                    employee => employee.RoleId);

            return employeePositionsIds;
        }
    }
}