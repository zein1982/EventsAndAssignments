using EventsAndAssignments.Models.DTO.Response;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<OrganizationResponseDto> GetOrganizationById(Guid id);
        Task<IReadOnlyCollection<OrganizationResponseDto>> GetOrganizations(string? name);
    }
}