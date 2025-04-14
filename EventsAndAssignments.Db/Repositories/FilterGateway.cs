using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class FilterGateway : IFilterGateway
    {
        private readonly ApplicationDbContext _context;

        public FilterGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<Employee?> GetResponsibleLeaders()
        {
            return _context.Assignments
                .AsNoTracking()
                .Where(x => x.ResponsibleLeaderId != null)
                .Include(x => x.ResponsibleLeader)
                .Select(x => x.ResponsibleLeader)
                .Distinct()
                .ToList();
        }

        public IReadOnlyCollection<Employee?> GetResponsibleExecutors()
        {
            return _context.Assignments
                .AsNoTracking()
                .Where(x => x.ResponsibleExecutor != null)
                .Include(x => x.ResponsibleExecutor)
                .Select(x => x.ResponsibleExecutor)
                .Distinct()
                .ToList();
        }

        public IReadOnlyCollection<Employee?> GetResponsibleInspectors()
        {
            return _context.Assignments
                .AsNoTracking()
                .Where(x => x.ResponsibleInspector != null)
                .Include(x => x.ResponsibleInspector)
                .Select(x => x.ResponsibleInspector)
                .Distinct()
                .ToList();
        }

        public IReadOnlyCollection<ProtocolFolder> GetProtocolFolders()
        {
            return _context
                .ProtocolFolders
                .NotRemoved()
                .ToList();
        }

        public IReadOnlyCollection<Employee?> GetAdministrators()
        {
            return _context.Employees
                .Where(x => x.RoleId == 2 || x.RoleId == 1).ToList();
        }
    }
}