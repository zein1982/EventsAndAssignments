using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using EventsAndAssignments.Services.Interfaces;
using MapsterMapper;

namespace EventsAndAssignments.Services.Data
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationGateway _organizationGateWay;
        private readonly IMapper _mapper;

        public OrganizationService(IOrganizationGateway organizationGateWay, IMapper mapper)
        {
            _organizationGateWay = organizationGateWay;
            _mapper = mapper;
        }

        public async Task<OrganizationResponseDto> GetOrganizationById(Guid id)
        {
            Organization organization =  await _organizationGateWay.GetOrganizationByIdAsync(id);
            OrganizationResponseDto response = _mapper.Map<OrganizationResponseDto>(organization);

            return response;
        }

        public async Task<IReadOnlyCollection<OrganizationResponseDto>> GetOrganizations(string? name)
        {
            IReadOnlyCollection<Organization> organizations = await _organizationGateWay.GetOrganizations(name);
            IReadOnlyCollection<OrganizationResponseDto> response = _mapper.Map<IReadOnlyCollection<OrganizationResponseDto>>(organizations);

            return response;
        }
    }
}