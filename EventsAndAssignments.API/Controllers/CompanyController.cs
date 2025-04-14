using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Models.DTO.Response;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;

        public CompanyController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        /// <summary>
        /// Получить организацию по идентификатору
        /// </summary>
        [HttpPost]
        [Route(nameof(GetCompanyById))]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponseDto>>> GetCompanyById([Required] Guid id)
        {
            OrganizationResponseDto organization = await _organizationService.GetOrganizationById(id);

            return Ok(organization);
        }

        /// <summary>
        /// Получить список организаций согласно названию
        /// </summary>
        [HttpPost]
        [Route(nameof(GetAllCompanies))]
        public async Task<ActionResult<IReadOnlyCollection<OrganizationResponseDto>>> GetAllCompanies([FromBody] GetCompanyRequestDto? name)
        {
            name ??= new GetCompanyRequestDto { Name = string.Empty };

            IReadOnlyCollection<OrganizationResponseDto> organizations =
                await _organizationService.GetOrganizations(name!.Name);

            return Ok(organizations);
        }
    }
}