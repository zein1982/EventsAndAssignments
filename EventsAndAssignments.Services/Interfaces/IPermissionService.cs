namespace EventsAndAssignments.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<List<string>> GetPermissionsAsync(Guid positionId);
    }
}