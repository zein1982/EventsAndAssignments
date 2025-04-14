using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Sorts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.Db.Repositories
{
    public class ProtocolGateway : IProtocolGateway
    {
        private readonly ApplicationDbContext _context;

        public ProtocolGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<Protocol>> ArchiveProtocol(IReadOnlyCollection<long> ids)
        {
            IQueryable<Protocol> protocolsForRemove = _context.Protocols
                .Intersect(ids)
                .Include(x => x.Assignments);

            foreach (var protocol in protocolsForRemove)
            {
                //Если поручений в протоколе нет или они все удалены то удаляем и протокол
                if (protocol.Assignments.IsNullOrEmpty()
                    || protocol.Assignments.All(e => e.Removed is not null))
                {
                    protocol.Removed = DateTime.UtcNow;
                }
                else
                {
                    protocol.IsArchived = true;
                    foreach (var assignment in protocol.Assignments)
                    {
                        assignment.IsArchived = true;
                    }
                }
            }

            List<Protocol> response = protocolsForRemove.ToList();
            await _context.SaveChangesAsync();
            return response;
        }

        public async Task<Protocol> CreateAsync(Protocol protocol)
        {
            _context.Protocols.Add(protocol);
            await _context.SaveChangesAsync();
            return protocol;
        }

        public (ICollection<Protocol> items, int count) GetAll(RequestParams? filterParams, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            if (filterParams is null)
            {
                return (new List<Protocol>(), 0);
            }

            IQueryable<Protocol> protocols = filterParams.RoleId == 1 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1)
                ? _context.Protocols
                    .AsNoTracking()
                    .NotRemoved()
                    .Include(x => x.CreatedByNavigation)
                : _context.Protocols
                    .AsNoTracking()
                    .NotRemoved()
                    .Include(x => x.CreatedByNavigation)
                    .Include(x => x.Folder)
                    .ThenInclude(x => x!.AllowedEmployeesNavigation)
                    .Where(
                        protocol => protocol.Folder != null
                            && filterParams.PositionId != null
                            && (protocol.CreatedBy == filterParams.PositionId.Value
                                || protocol.Folder.CreatedBy == filterParams.PositionId
                                || (protocol.CreatedBy != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(protocol.CreatedBy.Value))
                                || (protocol.Folder.CreatedBy != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(protocol.Folder.CreatedBy.Value))
                                || protocol.Folder.AllowedEmployeesNavigation!
                                    .Any(e => e.PositionId == filterParams.PositionId.Value
                                        || currentEmployeeAllPositionsWithRoles.Keys.Contains(e.PositionId))));//проверяю есть ли среди всех остальных должностей доступ к папке
            //присутствует в списках доступа у папки

            if (filterParams.ParentId is not null)
            {
                protocols = protocols.Where(x => x.FolderId == filterParams.ParentId);
            }

            foreach (var filter in filterParams.Filters)
            {
                //Отсеиваем согласно фильтрам
                List<string> filterSelectedValues = filter.Items!.Where(i => i.Selected).Select(i => i.Value).ToList();
                protocols = filter.Name switch
                {
                    nameof(Protocol.Created) => AddProtocolDateCreateFilter(protocols, filterSelectedValues),
                    nameof(Protocol.Updated) => AddProtocolDateUpdateFilter(protocols, filterSelectedValues),
                    _ => protocols
                };
            }

            int count = protocols.Count();

            List<Protocol> response = protocols
                .OrderBy(x => x.IsArchived)
                .ThenBySorted(filterParams.Sorts)
                .GetPage(filterParams.Count, filterParams.Page)
                .ToList();

            return (response, count);
        }

        public async Task<int> GetProtocolCountInFolder(long folderId)
        {
            int count = await _context.Protocols
                .Where(x => x.FolderId == folderId)
                .CountAsync();
            return count;
        }

        public async Task<Protocol> UpdateProtocolAsync(long protocolId, string name, Guid currentUserPositionId)
        {
            Protocol protocol = await _context.Protocols.SingleAsync(x => x.Id == protocolId);
            protocol.Name = name;
            protocol.UpdatedBy = currentUserPositionId;
            await _context.SaveChangesAsync();
            return protocol;
        }

        public async Task<IReadOnlyCollection<Assignment>> GetShortReportData(long id)
        {
            return await _context.Assignments
                 .AsNoTracking()
                 .Include(x => x.Comments)
                 .Include(x => x.Status)
                 .Include(x => x.ResponsibleLeader)
                 .Where(x => x.ProtocolId == id)
                 .ToListAsync();
        }

        public IReadOnlyCollection<Assignment> GetDataForByProtocolReport(List<long> ids)
        {
            return _context.Assignments
                .Intersect(ids)
                .Include(x => x.Status)
                .Include(x => x.Organization)
                .Include(x => x.Author)
                .Include(x => x.History)
                .Include(x => x.ResponsibleExecutor)
                .Include(x => x.ResponsibleLeader)
                .Include(x => x.ResponsibleInspector)
                .Include(x => x.Comments)
                .Include(x => x.Protocol)
                .ThenInclude(x => x.Folder)
                .OrderBy(x => x.Created)
                .ToList();
        }

        private IQueryable<Protocol> AddProtocolDateCreateFilter(IQueryable<Protocol> protocols, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return protocols;
            }

            if (filters.Count != 1)
            {
                throw new InvalidOperationException(
                    "Количество фильтров при фильтрации по дате должно быть равно 1");
            }

            string[] dates = filters[0].Split(',');

            List<DateTime> protocolDateCreate = dates
                .Select(DateTime.Parse).ToList();

            return protocols
                .Where(x => protocolDateCreate[0] <= x.Created
                    && x.Created <= protocolDateCreate[1]);
        }

        private IQueryable<Protocol> AddProtocolDateUpdateFilter(IQueryable<Protocol> protocols, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return protocols;
            }

            if (filters.Count != 1)
            {
                throw new InvalidOperationException(
                    "Количество фильтров при фильтрации по дате должно быть равно 1");
            }

            string[] dates = filters[0].Split(',');

            List<DateTime> protocolDateCreate = dates
                .Select(DateTime.Parse).ToList();

            return protocols
                .Where(x => protocolDateCreate[0] <= x.Updated
                    && x.Updated <= protocolDateCreate[1]);
        }
    }
}