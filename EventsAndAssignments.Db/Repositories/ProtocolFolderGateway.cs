using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Exceptions;
using EventsAndAssignments.Services.Sorts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.Db.Repositories
{
    public class ProtocolFoldersGateway : IProtocolFoldersGateway
    {
        readonly ApplicationDbContext _context;

        public ProtocolFoldersGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateProtocolFolderAsync(string folderName, Guid currentEmployeeId, long currentEmployeeRoleId, ICollection<Guid> allowedEmployeesIds)
        {
            ProtocolFolder folder = new()
            {
                Name = folderName,
                CreatedBy = currentEmployeeId,
                UpdatedBy = currentEmployeeId
            };

            //Если текущий пользователь создатель папки или администратор сисстемы то он может изменять пользователей папки
            if (folder.CreatedBy == currentEmployeeId || currentEmployeeRoleId is 1)
            {
                List<Employee> allowedEmployees = _context.Employees
                    .Where(employee => allowedEmployeesIds.Contains(employee.PositionId))
                    .ToList();

                folder.AllowedEmployeesNavigation = allowedEmployees;
            }

            _context.ProtocolFolders.Add(folder);

            int countUpdatedEntities = await _context.SaveChangesAsync();

            return countUpdatedEntities > 0;
        }

        public async Task<bool> UpdateProtocolFolderAsync(long id, string folderName, Guid folderOwner, Guid currentEmployeeId, long currentEmployeeRoleId,
            ICollection<Guid> allowedEmployeesIds, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            ProtocolFolder folder = await _context.ProtocolFolders
                .Include(x=>x.CreatedByNavigation)
                .Include(x=>x.UpdatedByNavigation)
                .Include(folder => folder.AllowedEmployeesNavigation)
                .GetById(id);

            if (folder.IsArchived)
            {
                throw new InvalidOperationException("Папка доступна только для чтения");
            }

            folder.Name = folderName;
            folder.UpdatedBy = currentEmployeeId;

            //Изменять владельца папки может только администратор системы
            if ((currentEmployeeRoleId is 1
                || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1)) //проверяю все должности текущего пользователя на наличие роли супер админа
                && folder.CreatedBy != folderOwner)
            {
                folder.CreatedByNavigation = null;
                folder.CreatedBy = folderOwner;
            }

            //Если текущий пользователь создатель папки или администратор системы, то он может изменять пользователей папки
            if (folder.CreatedBy == currentEmployeeId
                || currentEmployeeRoleId is 1
                || HasPositionWithAccess(folder.CreatedBy, currentEmployeeAllPositionsWithRoles))
            {
                List<Employee> allowedEmployees = _context.Employees
                    .Where(employee => allowedEmployeesIds.Contains(employee.PositionId))
                    .ToList();

                folder.AllowedEmployeesNavigation = null;
                folder.AllowedEmployeesNavigation = allowedEmployees;
            }

            int countUpdatedEntities = await _context.SaveChangesAsync();

            return countUpdatedEntities > 0;
        }

        public Task<ProtocolFolder> GetProtocolFolderAsync(long id) =>
            _context.ProtocolFolders
                .Include(folder => folder.CreatedByNavigation)
                .Include(folder => folder.AllowedEmployeesNavigation)
                .AsNoTracking()
                .FirstAsync(folder => folder.Id == id && folder.Removed == null);

        public Task<string> GetFolderName(long folderId) =>
            _context.ProtocolFolders
                .AsNoTracking()
                .Where(f => f.Id == folderId)
                .Select(f => f.Name)
                .FirstAsync();

        /// <summary>
        /// Получить пользователей имеющих доступ  к папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки</param>
        public async Task<IReadOnlyCollection<Employee>> GetEmployeesAllowedToFolderAsync(long folderId)
        {
            ProtocolFolder? folder = await _context.ProtocolFolders
                .Include(e => e.AllowedEmployeesNavigation)
                .FirstOrDefaultAsync(e => e.Id == folderId);

            return (folder!.AllowedEmployeesNavigation is null)
                ? new List<Employee>()
                : folder.AllowedEmployeesNavigation.ToList();
        }

        /// <summary>
        /// Добавить пользователя в список пользователей имеющих доступ к текущей папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки к которой предоставляется доступ</param>
        /// <param name="employeeId">Идентификатор пользователя, которому предоставляется доступ к папке</param>
        /// <param name="currentEmployeeId">Текущий пользователь системы</param>
        /// <param name="currentEmployeeRoleId">Идентификатор роли текущего пользователя системы</param>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<bool> AddAllowedEmployeeAsync(long folderId, Guid employeeId, Guid currentEmployeeId, long currentEmployeeRoleId)
        {
            //Получаю папку
            ProtocolFolder currentFolder = await _context.ProtocolFolders
                .Include(e => e.AllowedEmployeesNavigation)
                .AsNoTracking()
                .FirstAsync(folder => folder.Id == folderId && folder.Removed == null);

            Employee employeeToAdd = await _context.Employees.FindAsync(employeeId) ?? throw new  EntityNotFoundException(employeeId);

            //Если текущий пользователь не создатель папки или не администратор системы, то он не может добавлять к ней пользователей
            if (currentFolder.CreatedBy != currentEmployeeId && currentEmployeeRoleId is not 1)
            {
                return false;
            }

            //Если список равен null то создаем новый и добавляем пользователя иначе просто добавляем нового пользователя
            if (currentFolder.AllowedEmployeesNavigation is null)
            {
                currentFolder.AllowedEmployeesNavigation = new List<Employee> { employeeToAdd };
            }
            else
            {
                currentFolder.AllowedEmployeesNavigation.Add(employeeToAdd);
            }

            //Обновляем записи в БД
            int countUpdatedEntities = await _context.SaveChangesAsync();

            //Если количество обновленных записей больше 0 то возвращаем true иначе false
            return countUpdatedEntities > 0;
        }

        /// <summary>
        /// Удалить пользователя из списка пользователей имеющих доступ к текущей папке
        /// </summary>
        /// <param name="folderId">Идентификатор папки из которой исключается пользователь</param>
        /// <param name="employeeId">Идентификатор пользователя, у которого исключается доступ к папке</param>
        /// <param name="currentEmployeeId">Текущий пользователь системы</param>
        /// <param name="currentEmployeeRoleId">Идентификатор роли текущего пользователя системы</param>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<bool> RemoveAllowedEmployeeAsync(long folderId, Guid employeeId, Guid currentEmployeeId, long currentEmployeeRoleId)
        {
            //Получаю папку
            ProtocolFolder currentFolder = await _context.ProtocolFolders
                .Include(e => e.AllowedEmployeesNavigation)
                .AsNoTracking()
                .FirstAsync(folder => folder.Id == folderId && folder.Removed == null);

            Employee employeeToDelete = await _context.Employees.FindAsync(employeeId) ?? throw new  EntityNotFoundException(employeeId);

            //Если текущий пользователь не создатель папки то он не может удалять из нее пользователей
            if (currentFolder.CreatedBy != currentEmployeeId && currentEmployeeRoleId is not 1)
            {
                return false;
            }

            //Если список пользователей пуст то возвращаю результат о неудачном удалении пользователя 
            if (currentFolder.AllowedEmployeesNavigation is null)
            {
                return false;
            }

            //Если такого пользователя в списках нет то возвращаю результат о неудачном удалении
            if (!currentFolder.AllowedEmployeesNavigation.Contains(employeeToDelete))
            {
                return false;
            }

            //Удаляю пользователя из списка доступа к папке
            currentFolder.AllowedEmployeesNavigation.Remove(employeeToDelete);

            //Обновляем записи в БД
            int countUpdatedEntities = await _context.SaveChangesAsync();

            //Если количество обновленных записей больше 0 то возвращаем true иначе false
            return countUpdatedEntities > 0;
        }

        public async Task<(IReadOnlyCollection<ProtocolFolder>, int count)> GetProtocolFoldersAsync(RequestParams filter, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles)
        {
            IQueryable<ProtocolFolder> folders = filter.RoleId == 1 || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1)
                ? _context.ProtocolFolders
                    .Include(x => x.CreatedByNavigation)
                    .AsNoTracking()
                    .GetByFilter(filter)
                : _context.ProtocolFolders
                    .Include(x => x.CreatedByNavigation)
                    .AsNoTracking()
                    .Include(e => e.AllowedEmployeesNavigation)
                    .AsNoTracking()
                    .Where(x => //Показывать папку только создателю и пользователям из списка доступа
                        x.CreatedBy != null
                            && filter.PositionId != null
                            && (x.CreatedBy == filter.PositionId
                                || currentEmployeeAllPositionsWithRoles.Keys.Contains(x.CreatedBy.Value)
                                || x.AllowedEmployeesNavigation!
                                    .Any(e => e.PositionId == filter.PositionId.Value
                                        || currentEmployeeAllPositionsWithRoles.Keys
                                            .Contains(e
                                                .PositionId)))) //проверяю есть ли среди всех остальных должностей доступ к папке
                    .GetByFilter(filter);

            int folderCount = folders.Count();

            IReadOnlyCollection <ProtocolFolder> sortedFolders = await folders
                .OrderBy(x => x.IsArchived)
                .ThenBySorted(filter.Sorts)
                .GetPage(filter.Count, filter.Page)
                .ToListAsync();

            return (sortedFolders, folderCount);
        }

        public async Task ArchiveProtocolFolderAsync(List<long> idList)
        {
            IQueryable<ProtocolFolder> items = _context.ProtocolFolders.Where(x => idList.Contains(x.Id));

            if (!items.Any())
            {
                throw new EntityNotFoundException("Элементов не найденно");
            }

            foreach (var item in items)
            {
                item.IsArchived = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyCollection<ProtocolFolder>> RemoveProtocolFolderAsync(IReadOnlyCollection<long> idList)
        {
            IQueryable<ProtocolFolder> folders = _context.ProtocolFolders.NotRemoved().Where(x => idList.Contains(x.Id))
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.Protocols)
                .ThenInclude(x => x.Assignments);

            if (!folders.Any())
            {
                throw new EntityNotFoundException("Элементов не найденно");
            }

            foreach (var folder in folders)
            {
                //если в папке нет протоколов
                if (folder.Protocols.Count == 0
                    || folder.Protocols.All(x => x.IsArchived || x.Removed is not null))
                {
                    folder.Removed = DateTime.UtcNow;
                }
                else
                {
                    folder.IsArchived = true;
                    foreach (var protocol in folder.Protocols)
                    {
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
                }
            }

            List<ProtocolFolder> response = folders.ToList();

            await _context.SaveChangesAsync();

            return response;
        }

        /// <summary>
        /// Проверка того что одна из должностей текущего пользователя имеет права
        /// администратора системы либо папка была создана под одной из должностей
        /// </summary>
        /// <param name="idWithAccess">Id пользователя имеющего доступ к ресурсу</param>
        /// <param name="currentEmployeeAllPositionsWithRoles">Список Id должностей пользователя с ролями</param>
        private bool HasPositionWithAccess(Guid? idWithAccess, IReadOnlyDictionary<Guid, long?> currentEmployeeAllPositionsWithRoles) =>
            idWithAccess is not null
                && (currentEmployeeAllPositionsWithRoles.ContainsKey(idWithAccess.Value)
                    || currentEmployeeAllPositionsWithRoles.Values.Any(role => role == 1));
    }
}