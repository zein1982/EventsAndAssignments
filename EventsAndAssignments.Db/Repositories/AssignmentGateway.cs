using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Sorts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.Db.Repositories
{
    public class AssignmentsGateway : IAssignmentsGateway
    {
        private readonly ApplicationDbContext _context;

        public AssignmentsGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Assignment?> GetAssignmentByIdAsync(long id)
        {
            Assignment? assignment = await _context.Assignments
                .Include(e => e.Status)
                .Include(e => e.Organization)
                .Include(e => e.CreatedByNavigation)
                .Include(e => e.Author)
                .Include(e => e.ResponsibleLeader)
                .Include(e => e.ResponsibleExecutor)
                .Include(e => e.ResponsibleInspector)
                .Include(e => e.Comments)
                .Include(e => e.PeriodicNotifications)
                .Include(e => e.Protocol)
                .ThenInclude(e => e!.Folder)
                .ThenInclude(e=>e.AllowedEmployeesNavigation)
                .Select(assignment => new Assignment
                {
                    Id = assignment.Id,
                    Created = assignment.Created,
                    CreatedBy = assignment.CreatedBy,
                    Updated = assignment.Updated,
                    UpdatedBy = assignment.UpdatedBy,
                    Removed = assignment.Removed,
                    ExecutionDate = assignment.ExecutionDate,
                    LeaderExecutionDate = assignment.LeaderExecutionDate,
                    ExecutorExecutionDate = assignment.ExecutorExecutionDate,
                    InspectorCheckDate = assignment.InspectorCheckDate,
                    CompletionDate = assignment.CompletionDate,
                    GroupId = assignment.GroupId,
                    Name = assignment.Name,
                    Description = assignment.Description,
                    Subversion = assignment.Subversion,
                    Version = assignment.Version,
                    IsArchived = assignment.IsArchived,
                    ProtocolId = assignment.ProtocolId,
                    Protocol = assignment.Protocol,
                    StatusId = assignment.StatusId,
                    Status = assignment.Status,
                    OrganizationId = assignment.OrganizationId,
                    Organization = assignment.Organization,
                    AuthorId = assignment.AuthorId,
                    ResponsibleLeaderId = assignment.ResponsibleLeaderId,
                    ResponsibleExecutorId = assignment.ResponsibleExecutorId,
                    ResponsibleInspectorId = assignment.ResponsibleInspectorId,
                    History = assignment.History,
                    PeriodicNotifications = assignment.PeriodicNotifications,
                    Comments = assignment.Comments,
                    CreatedByNavigation = assignment.CreatedByNavigation != null
                        ? new Employee
                        {
                            PositionId = assignment.CreatedByNavigation.PositionId,
                            FirstName = assignment.CreatedByNavigation.FirstName,
                            MiddleName = assignment.CreatedByNavigation.MiddleName,
                            LastName = assignment.CreatedByNavigation.LastName,
                            Email = assignment.CreatedByNavigation.Email,
                            PositionName = assignment.CreatedByNavigation.PositionName,
                            DepartmentName = assignment.CreatedByNavigation.DepartmentName,
                            OrganizationName = assignment.CreatedByNavigation.OrganizationName,
                            TabelNumber = assignment.CreatedByNavigation.TabelNumber,
                        }
                    : null,
                    UpdatedByNavigation = assignment.UpdatedByNavigation,
                    Author = assignment.Author != null
                      ? new Employee
                      {
                          PositionId = assignment.Author.PositionId,
                          FirstName = assignment.Author.FirstName,
                          MiddleName = assignment.Author.MiddleName,
                          LastName = assignment.Author.LastName,
                          Email = assignment.Author.Email,
                          PositionName = assignment.Author.PositionName,
                          DepartmentName = assignment.Author.DepartmentName,
                          OrganizationName = assignment.Author.OrganizationName,
                          TabelNumber = assignment.Author.TabelNumber,
                      }
                      : null,
                    ResponsibleLeader = assignment.ResponsibleLeader != null
                      ?  new Employee
                      {
                          PositionId = assignment.ResponsibleLeader.PositionId,
                          FirstName = assignment.ResponsibleLeader.FirstName,
                          MiddleName = assignment.ResponsibleLeader.MiddleName,
                          LastName = assignment.ResponsibleLeader.LastName,
                          Email = assignment.ResponsibleLeader.Email,
                          PositionName = assignment.ResponsibleLeader.PositionName,
                          DepartmentName = assignment.ResponsibleLeader.DepartmentName,
                          OrganizationName = assignment.ResponsibleLeader.OrganizationName,
                          TabelNumber = assignment.ResponsibleLeader.TabelNumber,
                      }
                    : null,
                    ResponsibleExecutor =  assignment.ResponsibleExecutor != null
                       ? new Employee
                       {
                           PositionId = assignment.ResponsibleExecutor.PositionId,
                           FirstName = assignment.ResponsibleExecutor.FirstName,
                           MiddleName = assignment.ResponsibleExecutor.MiddleName,
                           LastName = assignment.ResponsibleExecutor.LastName,
                           Email = assignment.ResponsibleExecutor.Email,
                           PositionName = assignment.ResponsibleExecutor.PositionName,
                           DepartmentName = assignment.ResponsibleExecutor.DepartmentName,
                           OrganizationName = assignment.ResponsibleExecutor.OrganizationName,
                           TabelNumber = assignment.ResponsibleExecutor.TabelNumber,
                       }
                       : null,
                    ResponsibleInspector = assignment.ResponsibleInspector != null
                        ? new Employee
                        {
                            PositionId = assignment.ResponsibleInspector.PositionId,
                            FirstName = assignment.ResponsibleInspector.FirstName,
                            MiddleName = assignment.ResponsibleInspector.MiddleName,
                            LastName = assignment.ResponsibleInspector.LastName,
                            Email = assignment.ResponsibleInspector.Email,
                            PositionName = assignment.ResponsibleInspector.PositionName,
                            DepartmentName = assignment.ResponsibleInspector.DepartmentName,
                            OrganizationName = assignment.ResponsibleInspector.OrganizationName,
                            TabelNumber = assignment.ResponsibleInspector.TabelNumber,
                        }
                    : null,
                })
                .NotRemoved()
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (assignment is not null)
            {
                assignment.Files = GetRelatedAssignmentsFilesWithoutData(id);
            }

            return assignment;
        }

        public async Task<Assignment?> GetAssignmentByGroupIdAndVersionAsync(
            long groupId,
            int version,
            int subversion)
        {
            Assignment? assignment = await _context.Assignments
                .Include(e => e.Status)
                .Include(e => e.Organization)
                .Include(e => e.Author)
                .Include(e => e.ResponsibleLeader)
                .Include(e => e.ResponsibleExecutor)
                .Include(e => e.ResponsibleInspector)
                .Include(e => e.Protocol)
                .ThenInclude(e => e!.Folder)
                .Select(assignment => new Assignment
                {
                    Id = assignment.Id,
                    Created = assignment.Created,
                    CreatedBy = assignment.CreatedBy,
                    Updated = assignment.Updated,
                    UpdatedBy = assignment.UpdatedBy,
                    Removed = assignment.Removed,
                    ExecutionDate = assignment.ExecutionDate,
                    LeaderExecutionDate = assignment.LeaderExecutionDate,
                    ExecutorExecutionDate = assignment.ExecutorExecutionDate,
                    InspectorCheckDate = assignment.InspectorCheckDate,
                    CompletionDate = assignment.CompletionDate,
                    GroupId = assignment.GroupId,
                    Name = assignment.Name,
                    Description = assignment.Description,
                    Subversion = assignment.Subversion,
                    Version = assignment.Version,
                    IsArchived = assignment.IsArchived,
                    ProtocolId = assignment.ProtocolId,
                    Protocol = assignment.Protocol,
                    StatusId = assignment.StatusId,
                    Status = assignment.Status,
                    OrganizationId = assignment.OrganizationId,
                    Organization = assignment.Organization,
                    AuthorId = assignment.AuthorId,
                    ResponsibleLeaderId = assignment.ResponsibleLeaderId,
                    ResponsibleExecutorId = assignment.ResponsibleExecutorId,
                    ResponsibleInspectorId = assignment.ResponsibleInspectorId,
                    History = assignment.History,
                    Comments = assignment.Comments,
                    CreatedByNavigation = assignment.CreatedByNavigation,
                    UpdatedByNavigation = assignment.UpdatedByNavigation,
                    Author = assignment.Author != null
                      ? new Employee
                      {
                          PositionId = assignment.Author.PositionId,
                          FirstName = assignment.Author.FirstName,
                          MiddleName = assignment.Author.MiddleName,
                          LastName = assignment.Author.LastName,
                          Email = assignment.Author.Email,
                          PositionName = assignment.Author.PositionName,
                          DepartmentName = assignment.Author.DepartmentName,
                          OrganizationName = assignment.Author.OrganizationName,
                          TabelNumber = assignment.Author.TabelNumber,
                      }
                      : null,
                    ResponsibleLeader = assignment.ResponsibleLeader != null
                      ?  new Employee
                      {
                          PositionId = assignment.ResponsibleLeader.PositionId,
                          FirstName = assignment.ResponsibleLeader.FirstName,
                          MiddleName = assignment.ResponsibleLeader.MiddleName,
                          LastName = assignment.ResponsibleLeader.LastName,
                          Email = assignment.ResponsibleLeader.Email,
                          PositionName = assignment.ResponsibleLeader.PositionName,
                          DepartmentName = assignment.ResponsibleLeader.DepartmentName,
                          OrganizationName = assignment.ResponsibleLeader.OrganizationName,
                          TabelNumber = assignment.ResponsibleLeader.TabelNumber,
                      }
                    : null,
                    ResponsibleExecutor =  assignment.ResponsibleExecutor != null
                       ? new Employee
                       {
                           PositionId = assignment.ResponsibleExecutor.PositionId,
                           FirstName = assignment.ResponsibleExecutor.FirstName,
                           MiddleName = assignment.ResponsibleExecutor.MiddleName,
                           LastName = assignment.ResponsibleExecutor.LastName,
                           Email = assignment.ResponsibleExecutor.Email,
                           PositionName = assignment.ResponsibleExecutor.PositionName,
                           DepartmentName = assignment.ResponsibleExecutor.DepartmentName,
                           OrganizationName = assignment.ResponsibleExecutor.OrganizationName,
                           TabelNumber = assignment.ResponsibleExecutor.TabelNumber,
                       }
                       : null,
                    ResponsibleInspector = assignment.ResponsibleInspector != null
                        ? new Employee
                        {
                            PositionId = assignment.ResponsibleInspector.PositionId,
                            FirstName = assignment.ResponsibleInspector.FirstName,
                            MiddleName = assignment.ResponsibleInspector.MiddleName,
                            LastName = assignment.ResponsibleInspector.LastName,
                            Email = assignment.ResponsibleInspector.Email,
                            PositionName = assignment.ResponsibleInspector.PositionName,
                            DepartmentName = assignment.ResponsibleInspector.DepartmentName,
                            OrganizationName = assignment.ResponsibleInspector.OrganizationName,
                            TabelNumber = assignment.ResponsibleInspector.TabelNumber,
                        }
                    : null,
                })
                .AsNoTracking()
                .AsNoTracking()
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                x => x.GroupId == groupId
                    && x.Version == version
                    && x.Subversion == subversion);

            if (assignment is not null)
            {
                assignment.Files = GetRelatedAssignmentsFilesWithoutData(assignment.Id);
            }

            return assignment;
        }

        public async Task<int> GetAssignmentCountAsync(Func<Assignment, bool>? predicate = null)
        {
            return predicate is null
                ? await _context.Assignments.Where(x => predicate!(x)).CountAsync()
                :
                await _context.Assignments.CountAsync();
        }

        public List<long> GetFilteredAssignmentsIds(
            RequestParams filterParam,
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            IQueryable<Assignment> assignments = AssignmentsSortFiltration(filterParam, currentEmployeeAllPositionsWithRoles);

            List<long> result = assignments.Select(e => e.Id).ToList();

            return result;
        }

        public (List<Assignment> items, int count) GetFilteredAssignments(
            RequestParams filterParam,
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles,
            bool noPagination = false)
        {
            IQueryable<Assignment> assignments = AssignmentsSortFiltration(filterParam, currentEmployeeAllPositionsWithRoles);

            int count = assignments.Count();

            List<Assignment> result  = (filterParam.ParentId is not null)
                ? assignments //сортировка в протоколе
                    .AsNoTracking()
                    .OrderBy(e => e.IsArchived)
                    .ThenBySorted(filterParam.Sorts)
                    .ThenBy(e => e.GroupId)
                    .ThenBy(e => e.Subversion)
                    .GetPage(filterParam.Count, filterParam.Page)
                    .ToList()
                : assignments //сортировка во вкладке все поручения
                    .AsNoTracking()
                    .OrderBy(e => e.IsArchived)
                    .ThenBy(e => e.StatusId)
                    .ThenBySorted(filterParam.Sorts)
                    .ThenBy(e => e.GroupId)
                    .ThenBy(e => e.Subversion)
                    .GetPage(filterParam.Count, filterParam.Page)
                    .ToList();

            return (result, count);
        }

        public IReadOnlyCollection<Assignment> GetAssignmentForExcelReport(long protocolId)
        {
            return _context.Assignments
                .AsNoTracking()
                .NotRemoved()
                .Where(x => x.ProtocolId == protocolId)
                .Include(x => x.Status)
                .Include(e => e.History)
                .Include(x => x.ResponsibleLeader)
                .Include(x => x.Protocol)
                .ThenInclude(prot => prot!.Folder)
                .Include(x => x.Comments!.OrderByDescending(x => x.Created).Take(1))
                .OrderBy(e => e.GroupId)
                .ThenBy(e => e.Subversion)
                .ToList();
        }

        public async Task<ICollection<Assignment>> GetAssignmentsByGroupId(long groupId)
        {
            return await _context.Assignments
                .Include(e => e.Status)
                .Where(e => e.GroupId == groupId)
                .OrderBy(e => e.Version)
                .ThenBy(e => e.Subversion)
                .ToListAsync();
        }

        public long GetAssignmentsCountInProtocol(long protocolId)
        {
            return _context.Assignments
                .NotRemoved()
                .Count(x => x.ProtocolId == protocolId);
        }

        public async Task<Assignment> CreateAssignmentAsync(Assignment assignment)
        {
            //Создание
            await _context.Assignments.AddAsync(assignment);
            await _context.SaveChangesAsync();

            //Получение созданной сущности с заполненными навигационными данными
            Assignment created = await _context.Assignments
                .Include(e => e.Organization)
                .Include(e => e.Status)
                .Include(e => e.Protocol)
                .ThenInclude(e => e!.Folder)
                .SingleAsync(e => e.Id == assignment.Id);

            return created;
        }

        public async Task<Assignment?> GetAssignmentByIdWithFilesAsync(long id)
        {
            Assignment? assignment = await _context.Assignments
                .AsNoTracking()
                .Include(e => e.Protocol)
                .Include(e => e.Status)
                .Include(e => e.Comments)
                .AsNoTracking()
                .Include(e => e.Files)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id);
            return assignment;
        }

        public async Task<Assignment> UpdateAssignmentAsync(Assignment assignment)
        {
            assignment.CreatedByNavigation = null;
            assignment.ResponsibleExecutor = null;
            assignment.ResponsibleInspector = null;
            assignment.ResponsibleLeader = null;
            assignment.Author = null;
            assignment.UpdatedByNavigation = null;
            if (assignment.Protocol is not null)
            {
                assignment.Protocol.Folder = null;
            }

            if (assignment.Files is not null)
            {
                foreach (var assignmentFile in assignment.Files)
                {
                    assignmentFile.CreatedByNavigation = null;
                    assignmentFile.UpdatedByNavigation = null;
                }
            }

            _context.ChangeTracker.Clear();
            _context.Update(assignment);
            await _context.SaveChangesAsync();

            return assignment;
        }

        public async Task RemoveAssignmentsAsync(IReadOnlyCollection<long> ids)
        {
            IQueryable<Assignment> assignments = _context.Assignments.NotRemoved().Intersect(ids);

            if (ids.Count > assignments.Count())
            {
                throw new EntityNotFoundException();
            }

            long protocolId = assignments.OrderBy(e => e.Name).Last().ProtocolId;
            await assignments.ForEachAsync(x => x.Removed = DateTime.UtcNow);
            await _context.SaveChangesAsync();

            await RenameAndOrderAllAssignmentsInProtocol(protocolId);
        }

        public async Task RenameAndOrderAllAssignmentsInProtocol(long protocolId)
        {
            //Переназначение имен
            int newNumberAssignmentInProtocol = 0;
            List<Assignment> assignments = await _context.Assignments.NotRemoved()
                    .Where(e => e.ProtocolId == protocolId)
                .OrderBy(e => e.GroupId)
                .ThenBy(e => e.Subversion).ToListAsync();
            foreach (var assignment in assignments)
            {
                assignment.Name = assignment is { Subversion: 0 }
                    ? (++newNumberAssignmentInProtocol).ToString()
                    : newNumberAssignmentInProtocol.ToString() + $".{assignment.Subversion.ToString()}";
            }

            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<AssignmentStatus>> GetAllAssignmentStatusesAsync(bool hasResponsibleInspector)
        {
            return hasResponsibleInspector
                ? await _context.AssignmentStatuses.AsNoTracking().ToListAsync()
                : await _context.AssignmentStatuses.AsNoTracking().Where(x => x.IsInShortLine).ToListAsync();
        }

        public async Task<AssignmentStatus> GetAssignmentStatusByStatusCode(int statusCode)
        {
            AssignmentStatus status = await _context.AssignmentStatuses
                .AsNoTracking()
                .SingleAsync(x => x.StatusCode == statusCode);
            return status;
        }

        public IReadOnlyCollection<Assignment> GetAssignmentsForShortReport(List<long> ids)
        {
            return _context.Assignments
                .Intersect(ids)
                .Include(x => x.Status)
                .Include(x => x.Organization)
                .Include(e => e.Comments)
                .Include(e => e.ResponsibleLeader)
                .Include(e => e.Protocol)
                .OrderBy(e => e.GroupId)
                .ThenBy(e => e.Subversion)
                .ToList();
        }

        public async Task<int> GetEmployeeAssignmentCount(Guid positionId, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            int assignments = await _context.Assignments
                .NotRemoved()
                .Where(x =>
                    (x.ResponsibleLeaderId == positionId
                        || (x.ResponsibleLeaderId != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleLeaderId.Value))
                        || x.ResponsibleExecutorId == positionId
                        || (x.ResponsibleExecutorId != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleExecutorId.Value))
                        || (x.ResponsibleInspectorId == positionId && x.StatusId == (long)Status.Monitoring)
                        || (x.ResponsibleInspectorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleInspectorId.Value)
                            && x.StatusId == (long)Status.Monitoring))
                        && !x.IsArchived
                        && x.StatusId >= (long)Status.InWork
                        && x.StatusId <= (long)Status.Completed)
                .CountAsync();
            return assignments;
        }

        public Task<int> GetEmployeeAssignmentUnFinishedCount(Guid positionId, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            IQueryable<Assignment> assignments = _context.Assignments
                .NotRemoved();
            return assignments
                .Where(x => (x.ResponsibleLeaderId == positionId
                    || (x.ResponsibleLeaderId != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleLeaderId.Value))
                    || x.ResponsibleExecutorId == positionId
                    || (x.ResponsibleExecutorId != null && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleExecutorId.Value))
                    || (x.ResponsibleInspectorId == positionId && x.StatusId == (long)Status.Monitoring)
                    || (x.ResponsibleInspectorId != null
                        && currentEmployeeAllPositionsWithRoles.Keys.Contains(x.ResponsibleInspectorId.Value)
                        && x.StatusId == (long)Status.Monitoring))
                    && !x.IsArchived
                    && x.StatusId >= (long)Status.InWork
                    && x.StatusId <= (long)Status.Completed
                    && x.ExecutionDate < DateTime.UtcNow.Date)
                .CountAsync();
        }

        public List<Guid> GetFolderAllowedEmployees(long id)
        {
            Assignment assignment = _context.Assignments
                .Include(assignment => assignment.Protocol)
                .ThenInclude(protocol => protocol!.Folder)
                .ThenInclude(protocolFolder => protocolFolder!.AllowedEmployeesNavigation)
                .First(e => e.Id == id);

            if (assignment.Protocol?.Folder?.AllowedEmployeesNavigation is null)
            {
                return new List<Guid>();
            }

            List<Guid> allowedEmployees = assignment.Protocol?.Folder?.AllowedEmployeesNavigation
                    .Select(e => e.PositionId)
                    .ToList()
                ?? new List<Guid>();

            if (assignment.Protocol is null)
            {
                return allowedEmployees;
            }

            if (assignment.Protocol.CreatedBy is not null)
            {
                allowedEmployees.Add(assignment.Protocol.CreatedBy.Value);
            }

            if (assignment.Protocol.Folder?.CreatedBy is not null)
            {
                allowedEmployees.Add(assignment.Protocol.Folder.CreatedBy.Value);
            }

            return allowedEmployees;
        }

        public Task<List<AssignmentFile>> GetRelatedAssignmentsFilesWithData(long assignmentId) =>
            _context.Files
                .Include(file => file.CreatedByNavigation)
                .Where(e => e.AssignmentId == assignmentId && e.Removed == null)
                .ToListAsync();

        private List<AssignmentFile> GetRelatedAssignmentsFilesWithoutData(long assignmentId) =>
            _context.Files
                .Include(file => file.CreatedByNavigation)
                .Where(e => e.AssignmentId == assignmentId && e.Removed == null)
                .Select(e => new AssignmentFile
                {
                    Id = e.Id,
                    OriginName = e.OriginName,
                    Created = e.Created,
                    CreatedBy = e.CreatedBy,
                    CreatedByNavigation = new Employee
                    {
                        PositionId = e.CreatedByNavigation!.PositionId,
                        FirstName = e.CreatedByNavigation.FirstName,
                        MiddleName = e.CreatedByNavigation.MiddleName,
                        LastName = e.CreatedByNavigation.LastName,
                        Email = e.CreatedByNavigation.Email,
                        PositionName = e.CreatedByNavigation.PositionName,
                        DepartmentName = e.CreatedByNavigation.DepartmentName,
                        OrganizationName = e.CreatedByNavigation.OrganizationName,
                        TabelNumber = e.CreatedByNavigation.TabelNumber,
                    },
                })
                .ToList();

        private IQueryable<Assignment> AddResponsibleManagerFilter(IQueryable<Assignment> assigments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assigments;
            }

            List<Guid?> managers = filters.ConvertAll(guid => Guid.Parse(guid) as Guid?);

            return assigments.Where(x => managers.Contains(x.ResponsibleLeaderId));
        }

        private IQueryable<Assignment> AddResponsibleExecutorFilter(
            IQueryable<Assignment> assigments,
            List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assigments;
            }

            List<Guid?> managers = filters.ConvertAll(guid => Guid.Parse(guid) as Guid?);

            return assigments.Where(x => managers.Contains(x.ResponsibleExecutorId));
        }

        private IQueryable<Assignment> AddResponsibleInspectorFilter(
            IQueryable<Assignment> assigments,
            List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assigments;
            }

            List<Guid?> managers = filters.ConvertAll(guid => Guid.Parse(guid) as Guid?);

            return assigments.Where(x => managers.Contains(x.ResponsibleInspectorId));
        }

        private IQueryable<Assignment> AddCompanyFilter(IQueryable<Assignment> assignments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assignments;
            }

            List<Guid?> companyIds = filters.ConvertAll(id => Guid.Parse(id) as Guid?);

            return assignments.Where(x => companyIds.Contains(x.OrganizationId));
        }

        private IQueryable<Assignment> AddAssignmentFolder(IQueryable<Assignment> assignments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assignments;
            }

            if (!long.TryParse(filters[0], out long test))
            {
                return assignments.Where(x => x.Protocol.Folder.Name.Contains(filters[0]));
            }

            List<long?> folderIds = filters.ConvertAll(id => long.Parse(id) as long?);

            return assignments
                .Where(x => folderIds.Contains(x.Protocol.FolderId));
        }

        private IQueryable<Assignment> AddStatusFilter(IQueryable<Assignment> assigments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assigments;
            }

            List<long?> statusesId = filters.ConvertAll(id => long.Parse(id) as long?);
            return assigments.Where(x => statusesId.Contains(x.StatusId));
        }

        private IQueryable<Assignment> AddProtocolNumberFilter(IQueryable<Assignment> assigments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assigments;
            }

            return assigments.Where(x => x.Protocol.Name.Contains(filters[0]));
        }

        private IQueryable<Assignment> AddUrgencyFilter(IQueryable<Assignment> assigments, List<string> filters)
        {
            if (filters.IsNullOrEmpty() || filters.Count >= 2)
            {
                return assigments;
            }

            assigments = assigments
                .Include(x => x.History);

            int urgency = Convert.ToInt32(filters[0]);
            //просрочка
            if (urgency == 0)
            {
                IQueryable<Assignment> completedbutlate = assigments
                    .Where(x => (x.StatusId == 7 && x.Updated!.Value.Date > x.ExecutionDate.Date)
                        || (x.ExecutionDate.Date < DateTime.UtcNow.Date && x.StatusId<7));
                return completedbutlate;
            }
            else
            {
                IQueryable<Assignment> completeInTime = assigments
                    .Where(x=> (x.StatusId==7 && x.Updated!.Value.Date <= x.ExecutionDate.Date)
                        || (x.ExecutionDate.Date >= DateTime.UtcNow.Date && x.StatusId<7));
                return completeInTime;
            }
        }

        //фильтрация по созданию
        private IQueryable<Assignment> AddAssignDateFilter(IQueryable<Assignment> assignments, List<string> filters)
        {
            if (filters.IsNullOrEmpty())
            {
                return assignments;
            }

            if (filters.Count != 1)
            {
                throw new InvalidOperationException(
                    "Количество фильтров при фильтрации по дате должно быть равно 1");
            }

            string[] dates = filters[0].Split(',');

            List<DateTime> protocolDateCreate = dates
                .Select(DateTime.Parse).ToList();

            return assignments
                .Where(x => protocolDateCreate[0] <= x.Created
                    && x.Created <= protocolDateCreate[1]);
        }

        private IQueryable<Assignment> GetCurrentEmployeeAssignments(Guid positionId
            , IQueryable<Assignment> assignments, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles) =>
            assignments.Where(
                assignment => //Показывать протоколы только создателю папки либо протокола и пользователям из списка доступа (ПО ВСЕМ ДОЛЖНОСТЯМ ТЕКУЩЕГО ПОЛЬЗОВАТЕЛЯ)
                    assignment.CreatedBy == positionId //является создателем поручения
                        || (assignment.CreatedBy != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.CreatedBy.Value))
                        || assignment.ResponsibleExecutorId == positionId //является Исполнителем поручения
                        || (assignment.ResponsibleExecutorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleExecutorId.Value))
                        || assignment.ResponsibleInspectorId == positionId //является Контролером поручения
                        || (assignment.ResponsibleInspectorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleInspectorId.Value))
                        || assignment.ResponsibleLeaderId == positionId //является Руководителем поручения
                        || (assignment.ResponsibleLeaderId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleLeaderId.Value))
                        || assignment.AuthorId == positionId //является Автором поручения
                        || (assignment.AuthorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.AuthorId.Value))
                        || (assignment.Protocol != null && assignment.Protocol.CreatedBy == positionId) //является создателем протокола
                        || (assignment.Protocol != null
                            && assignment.Protocol.CreatedBy != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.Protocol.CreatedBy.Value))
                        || (assignment.Protocol != null
                            && assignment.Protocol.Folder != null
                            && assignment.Protocol.Folder.CreatedBy == positionId) //является создателем папки
                        || (assignment.Protocol != null
                            && assignment.Protocol.Folder != null
                            && assignment.Protocol.Folder.CreatedBy != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.Protocol.Folder.CreatedBy.Value))
                        || assignment.Protocol!.Folder!.AllowedEmployeesNavigation! //присутствует в списках доступа у папки
                            .Any(e => e.PositionId == positionId
                                || currentEmployeeAllPositionsWithRoles.Keys.Contains(e.PositionId))
            );

        private IQueryable<Assignment> AddUserAssignmentFilter(IQueryable<Assignment> assignments, List<string> filters, RequestParams filterParam,
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            if (filters.IsNullOrEmpty())
            {
                return assignments;
            }

            IQueryable<Assignment> result = assignments;

            if (filters.Count == 2)
            {
                result = result.Where(assignment => filterParam.PositionId != null
                    && (assignment.ResponsibleExecutorId == filterParam.PositionId.Value //является Исполнителем поручения
                        || (assignment.ResponsibleExecutorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleExecutorId.Value))
                        || assignment.ResponsibleInspectorId == filterParam.PositionId.Value //является Контролером поручения
                        || (assignment.ResponsibleInspectorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleInspectorId.Value))
                        || assignment.ResponsibleLeaderId == filterParam.PositionId.Value //является Руководителем поручения
                        || (assignment.ResponsibleLeaderId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleLeaderId.Value)))
                    && (assignment.AuthorId == filterParam.PositionId.Value
                        || (assignment.AuthorId != null
                            && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.AuthorId.Value))));
            }
            else
            {
                foreach (var filter in filters)
                {
                    switch (filter)
                    {
                        case nameof(FilterItemValue.OnlyMine):
                            result = result.Where(assignment => filterParam.PositionId != null
                                && (assignment.ResponsibleExecutorId == filterParam.PositionId.Value //является Исполнителем поручения
                                    || (assignment.ResponsibleExecutorId != null
                                        && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleExecutorId.Value))
                                    || assignment.ResponsibleInspectorId == filterParam.PositionId.Value //является Контролером поручения
                                    || (assignment.ResponsibleInspectorId != null
                                        && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleInspectorId.Value))
                                    || assignment.ResponsibleLeaderId == filterParam.PositionId.Value //является Руководителем поручения
                                    || (assignment.ResponsibleLeaderId != null
                                        && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleLeaderId.Value)))
                                && assignment.AuthorId != filterParam.PositionId.Value
                                && ((assignment.AuthorId != null
                                    && !currentEmployeeAllPositionsWithRoles.Keys.Contains(
                                        assignment.AuthorId.Value))
                                    || assignment.AuthorId == null));
                            break;

                        case nameof(FilterItemValue.IamAuthor):
                            result = result.Where(assignment => filterParam.PositionId != null //НЕ является Автором
                                && (assignment.AuthorId == filterParam.PositionId.Value
                                    || (assignment.AuthorId != null
                                        && currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.AuthorId.Value)))
                                && assignment.ResponsibleExecutorId != filterParam.PositionId.Value // НЕ является Исполнителем поручения
                                && ((assignment.ResponsibleExecutorId != null
                                    && !currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleExecutorId.Value))
                                    || assignment.ResponsibleExecutorId == null)
                                && assignment.ResponsibleInspectorId != filterParam.PositionId.Value // НЕ является Контролером поручения
                                && ((assignment.ResponsibleInspectorId != null
                                    && !currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleInspectorId.Value))
                                    || assignment.ResponsibleInspectorId == null)
                                && assignment.ResponsibleLeaderId != filterParam.PositionId.Value //НЕ является Руководителем поручения
                                && ((assignment.ResponsibleLeaderId != null
                                    && !currentEmployeeAllPositionsWithRoles.Keys.Contains(assignment.ResponsibleLeaderId.Value))
                                    || assignment.ResponsibleLeaderId == null));
                            break;
                    }
                }
            }

            return result;
        }

        private IQueryable<Assignment> AssignmentsSortFiltration(RequestParams filterParam,
            IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            IQueryable<Assignment> assignments = _context.Assignments
                .AsNoTracking()
                .Include(assignment => assignment.Protocol)
                .ThenInclude(protocol => protocol!.Folder);

            if (filterParam.RoleId != 1 || currentEmployeeAllPositionsWithRoles.Values.All(role => role != 1))
            {
                if (filterParam.PositionId is not null)
                {
                    assignments = GetCurrentEmployeeAssignments(filterParam.PositionId.Value, assignments,
                        currentEmployeeAllPositionsWithRoles);
                }
            }

            assignments = assignments
                .Where(x => x.Removed == null)
                .Include(x => x.Comments!.Where(x => x.Removed == null)
                    .OrderByDescending(comment => comment.Created)
                    .Take(1))
                .Include(e => e.Status)
                .Include(e => e.Organization)
                .Include(e => e.Author)
                .Include(e => e.ResponsibleLeader)
                .Include(e => e.ResponsibleExecutor)
                .Include(e => e.ResponsibleInspector);

            if (filterParam.RoleId is 3)
            {
                assignments = assignments.Where(e => e.StatusId >= 3);
            }

            if (filterParam.ParentId is not null)
            {
                assignments = assignments.Where(x => x.ProtocolId == filterParam.ParentId);
            }

            foreach (var filter in filterParam.Filters)
            {
                //тут выбираем те фильтры, которые выбраны
                List<string> filterSelectedValues = filter.Items!.Where(i => i.Selected).Select(i => i.Value).ToList();
                assignments = filter.Name switch
                {
                    nameof(Assignment.Created) => AddAssignDateFilter(assignments, filterSelectedValues),//Дата назначения поручения
                    nameof(Assignment.CreatedBy) => AddResponsibleManagerFilter(assignments, filterSelectedValues),//фильтрация по создателю поручения
                    nameof(Assignment.Organization) => AddCompanyFilter(assignments, filterSelectedValues),//идентификатор компании
                    nameof(Assignment.Status) => AddStatusFilter(assignments, filterSelectedValues),//фильтрация по статусам
                    nameof(Assignment.Protocol) => AddProtocolNumberFilter(assignments, filterSelectedValues),//фильтрация по имени протокола
                    nameof(Assignment.ResponsibleLeaderId) => AddResponsibleManagerFilter(assignments, filterSelectedValues),//фильтрация по ответственному исполнителю
                    nameof(Assignment.ResponsibleInspector) => AddResponsibleInspectorFilter(assignments, filterSelectedValues),//фильтрация по контролеру
                    nameof(Assignment.ResponsibleExecutor) => AddResponsibleExecutorFilter(assignments, filterSelectedValues),//фильтрация по отв исполнителю
                    nameof(FilterFieldName.Urgency) => AddUrgencyFilter(assignments, filterSelectedValues),//фильтрация по просроченным
                    nameof(Assignment.Protocol.Folder) => AddAssignmentFolder(assignments, filterSelectedValues),
                    nameof(FilterFieldName.UserAssignment) => AddUserAssignmentFilter(assignments, filterSelectedValues, filterParam, currentEmployeeAllPositionsWithRoles),
                    nameof(FilterFieldName.Activity) => AddActivityFilter(assignments, filterSelectedValues), //фильтр по активности
                    _ => assignments
                };
            }

            return assignments;
        }

        private IQueryable<Assignment> AddActivityFilter(IQueryable<Assignment> assignments, List<string> filters)
        {
            if (filters.IsNullOrEmpty() || filters.Count >= 2)
            {
                return assignments;
            }

            assignments = assignments
                .Include(x => x.History);

            int activity = Convert.ToInt32(filters[0]);
            DateTime thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            return (activity == 0)
                ? assignments // Архивные
                    .Where(x => x.StatusId == 7 && x.CompletionDate.HasValue && x.CompletionDate.Value <= thirtyDaysAgo)
                : assignments // Активные
                    .Where(x => x.StatusId != 7 || !x.CompletionDate.HasValue || x.CompletionDate.Value > thirtyDaysAgo);
        }
    }
}