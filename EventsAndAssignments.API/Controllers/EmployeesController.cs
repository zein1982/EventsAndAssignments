using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EventsAndAssignments.Models.DTO;
using EventsAndAssignments.Models.DTO.Common;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _service;
        private readonly ILogger<EmployeesController> _logger;

        public EmployeesController(IEmployeeService service, ILogger<EmployeesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        /// <summary>
        /// Возвращает текущего сотрудника (по токену авторизации)
        /// </summary>
        [HttpGet]
        [Route(nameof(GetCurrentEmployee))]
        public async Task<ActionResult<Employee>> GetCurrentEmployee()
        {
            string? userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
            {
                return NotFound();
            }

            EmployeeWithAllPositionsDto? employee = await _service.GetEmployeeWithAllPositionsByEmail(userEmail);

            return employee is null ? NotFound() : Ok(employee);
        }

        /// <summary>
        /// Вернуть трудозанятых по совпадению в фамилии/имени/отчестве
        /// </summary>
        /// <param name="name">Аргумент фильтра, допускающий указание
        ///  пользовательских данных в последовательности "фамилия имя отчество"</param>
        /// <param name="count">Количество возвращаемых результатов</param>
        [HttpGet]
        [Route(nameof(GetEmployeesByName))]
        public ActionResult<IReadOnlyCollection<Employee>> GetEmployeesByName(
            [Required] string name, [Required] int count = 50)
        {
            IReadOnlyCollection<Employee> employees = _service.GetEmployeesByName(name, count);

            if (employees.IsNullOrEmpty())
            {
                return NoContent();
            }

            return Ok(employees);
        }

        [HttpPost]
        [Route(nameof(SetEmployeeRole))]
        public async Task<ActionResult<Employee>> SetEmployeeRole(Guid userId, long roleId)
        {
            return Ok(await _service.SetEmployeeRole(userId, roleId));
        }

        /// <summary>
        /// Вернуть трудозанятых по идентификатору
        /// </summary>
        [HttpGet]
        [Route(nameof(GetEmployeeById))]
        public ActionResult<IReadOnlyCollection<Employee>> GetEmployeeById([Required] Guid identifier)
        {
            Employee? employees = _service.GetEmployeeById(identifier);

            if (employees is null)
            {
                return NoContent();
            }

            return Ok(employees);
        }

        /// <summary>
        /// Возвращает фотографию пользователя по идентификатору
        /// </summary>
        /// <param name="id">Идентификатор сотрудника</param>
        /// <param name="isSmall">Тип фотографии (полный или уменьшенный вариант). По умолчанию уменьшенный</param>
        [HttpGet]
        [Route(nameof(GetEmployeePhotoById))]
        public async Task<ActionResult<Employee>> GetEmployeePhotoById([Required] Guid id, bool isSmall = true)
        {
            byte[]? photo = await _service.GetEmployeePhotoById(id, isSmall);

            return (photo is not null) ? File(photo, "image/png") : NoContent();
        }
    }
}