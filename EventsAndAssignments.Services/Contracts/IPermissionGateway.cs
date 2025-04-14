namespace EventsAndAssignments.Services.Contracts
{
    public interface IPermissionGateway
    {
        Task<List<string>> GetPermissionsAsync(Guid positionId);
    }
}