using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventsAndAssignments.Services.Data
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionGateway _permissionGateway;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            IPermissionGateway permissionGateway,
            ILogger<PermissionService> logger)
        {
            _permissionGateway = permissionGateway;
            _logger = logger;
        }

        public async Task<List<string>> GetPermissionsAsync(Guid positionId)
        {
            List<string> permissions =  await _permissionGateway.GetPermissionsAsync(positionId);

            if (permissions.Count is 0)
            {
                _logger.LogWarning("У пользователя с id: [{UserId}] список привелегий пуст", positionId);
            }

            return permissions;
        }
    }
}