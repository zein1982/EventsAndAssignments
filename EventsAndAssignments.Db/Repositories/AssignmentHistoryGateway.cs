using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EventsAndAssignments.Db.Repositories
{
    public class AssignmentHistoryGateway : IAssignmentHistoryGateway
    {
        private readonly ApplicationDbContext _context;

        public AssignmentHistoryGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AssignmentHistory> CreateAsync(AssignmentHistory history)
        {
            EntityEntry<AssignmentHistory> historyEntry = await _context.AddAsync(history);
            await _context.SaveChangesAsync();

            return historyEntry.Entity;
        }

        public async Task<ICollection<AssignmentHistory>> GetAllAsync(long? assignmentId)
        {
            ICollection<AssignmentHistory> items = await _context
                .AssignmentHistories
                .Where(history => history.AssignmentId == assignmentId
                    && history.ModificationType != (int)AssignmentModificationTypes.OpenAssignment)
                .OrderBy(history => history.Created)
                .Include(history => history.CreatedByNavigation)
                .Include(history => history.AddedResponsibleExecutorNavigation)
                .Include(history => history.RemovedResponsibleExecutorNavigation)
                .Include(history => history.FromStatusNavigation)
                .Include(history => history.ToStatusNavigation)
                .Include(history => history.AddedFileNavigation)
                .Include(history => history.RemovedFileNavigation)
                .Select(history => new AssignmentHistory
                {
                    Id = history.Id,
                    AssignmentId = history.AssignmentId,
                    ModificationType = history.ModificationType,
                    Created = history.Created,
                    CreatedBy = history.CreatedBy,
                    AddedResponsibleExecutor = history.AddedResponsibleExecutor,
                    RemovedResponsibleExecutor = history.RemovedResponsibleExecutor,
                    AddedFile = history.AddedFile,
                    RemovedFile = history.RemovedFile,
                    FromStatus = history.FromStatus,
                    ToStatus = history.ToStatus,
                    Assignment = history.Assignment,
                    FromStatusNavigation = history.FromStatusNavigation,
                    ToStatusNavigation = history.ToStatusNavigation,
                    CreatedByNavigation = history.CreatedByNavigation != null
                        ? new Employee
                        {
                            PositionId = history.CreatedByNavigation.PositionId,
                            FirstName = history.CreatedByNavigation.FirstName,
                            MiddleName = history.CreatedByNavigation.MiddleName,
                            LastName = history.CreatedByNavigation.LastName,
                            Email = history.CreatedByNavigation.Email,
                            PositionName = history.CreatedByNavigation.PositionName,
                            DepartmentName = history.CreatedByNavigation.DepartmentName,
                            OrganizationName = history.CreatedByNavigation.OrganizationName,
                            TabelNumber = history.CreatedByNavigation.TabelNumber,
                        }
                        : null,
                    AddedResponsibleExecutorNavigation = history.AddedResponsibleExecutorNavigation != null
                        ? new Employee
                        {
                            PositionId = history.AddedResponsibleExecutorNavigation.PositionId,
                            FirstName = history.AddedResponsibleExecutorNavigation.FirstName,
                            MiddleName = history.AddedResponsibleExecutorNavigation.MiddleName,
                            LastName = history.AddedResponsibleExecutorNavigation.LastName,
                            Email = history.AddedResponsibleExecutorNavigation.Email,
                            PositionName = history.AddedResponsibleExecutorNavigation.PositionName,
                            DepartmentName = history.AddedResponsibleExecutorNavigation.DepartmentName,
                            OrganizationName = history.AddedResponsibleExecutorNavigation.OrganizationName,
                            TabelNumber = history.AddedResponsibleExecutorNavigation.TabelNumber,
                        }
                        : null,
                    RemovedResponsibleExecutorNavigation = history.RemovedResponsibleExecutorNavigation != null
                        ? new Employee
                        {
                            PositionId = history.RemovedResponsibleExecutorNavigation.PositionId,
                            FirstName = history.RemovedResponsibleExecutorNavigation.FirstName,
                            MiddleName = history.RemovedResponsibleExecutorNavigation.MiddleName,
                            LastName = history.RemovedResponsibleExecutorNavigation.LastName,
                            Email = history.RemovedResponsibleExecutorNavigation.Email,
                            PositionName = history.RemovedResponsibleExecutorNavigation.PositionName,
                            DepartmentName = history.RemovedResponsibleExecutorNavigation.DepartmentName,
                            OrganizationName = history.RemovedResponsibleExecutorNavigation.OrganizationName,
                            TabelNumber = history.RemovedResponsibleExecutorNavigation.TabelNumber,
                        }
                        : null,
                    AddedFileNavigation = history.AddedFileNavigation != null
                    ? new AssignmentFile
                    {
                        Id = history.AddedFileNavigation.Id,
                        OriginName = history.AddedFileNavigation.OriginName,
                        SafetyName = history.AddedFileNavigation.SafetyName,
                        Created = history.AddedFileNavigation.Created,
                        CreatedBy = history.AddedFileNavigation.CreatedBy,
                    }
                    : null,
                    RemovedFileNavigation = history.RemovedFileNavigation != null
                        ? new AssignmentFile
                        {
                            Id = history.RemovedFileNavigation.Id,
                            OriginName = history.RemovedFileNavigation.OriginName,
                            SafetyName = history.RemovedFileNavigation.SafetyName,
                            Created = history.RemovedFileNavigation.Created,
                            CreatedBy = history.RemovedFileNavigation.CreatedBy,
                        }
                        : null,
                })
                .ToListAsync();

            return items;
        }
    }
}