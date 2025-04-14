using EventsAndAssignments.Services.Interfaces;
using EventsAndAssignments.Services.Sorts;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        private readonly IFilterService _filterService;

        public FilterController(IFilterService filterService)
        {
            _filterService = filterService;
        }

        [HttpGet]
        [Route(nameof(GetAssigmentFilters))]
        public async Task<ActionResult<IReadOnlyCollection<FieldFilter>>> GetAssigmentFilters()
        {
            return Ok(await _filterService.GetAssignmentFilters());
        }

        /// <summary>
        /// Фильтры для папок
        /// </summary>
        [HttpGet]
        [Route(nameof(GetFolderFilters))]
        public ActionResult<IReadOnlyCollection<FieldFilter>> GetFolderFilters()
        {
            return Ok(_filterService.GetFolderFilters());
        }

        [HttpGet]
        [Route(nameof(GetProtocolFilters))]
        public ActionResult<IReadOnlyCollection<FieldFilter>> GetProtocolFilters()
        {
            return Ok(_filterService.GetProtocolFilters());
        }

        [HttpGet]
        [Route(nameof(GetSorts))]
        public ActionResult<IReadOnlyCollection<string>> GetSorts()
        {
            return Ok(_filterService.GetSortsAssignmentName());
        }
    }
}