using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class PermissionGateway : IPermissionGateway
    {
        private readonly ApplicationDbContext _context;

        public PermissionGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetPermissionsAsync(Guid positionId)
        {
            Employee? employee = await _context.Employees
                .Include(x => x.UserRole)
                .ThenInclude(x => x.Permissions)
                .FirstOrDefaultAsync(x => x.PositionId == positionId);

            return employee?.UserRole is null
                ? new List<string>()
                : employee.UserRole.Permissions.Select(x => x.Name).ToList();
        }
    }
}