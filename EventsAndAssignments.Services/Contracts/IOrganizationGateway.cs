using EventsAndAssignments.Services.DAO;

namespace EventsAndAssignments.Services.Contracts
{
    public interface IOrganizationGateway
    {
        Task<Organization> GetOrganizationByIdAsync(Guid id);
        Task<Organization> CreateOrganization(Organization organization);
        Task<IReadOnlyCollection<Organization>> GetOrganizations(string? name);
    }
}