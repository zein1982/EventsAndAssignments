using System.ComponentModel.DataAnnotations;
using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventsAndAssignments.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CalendarController : Controller
    {
        private readonly ICalendarService _calendarService;

        public CalendarController(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpPost]
        [Route(nameof(GetHolidays))]
        public IActionResult GetHolidays([Required] DateRangeRequestDto dateRange)
        {
            if (dateRange.StartDate >= dateRange.EndDate)
            {
                return BadRequest("Дата начала периода не может быть больше или равняться дате окончания периода");
            }

            IReadOnlyList<DateOnly> holidayDate = _calendarService.GetHolidayDates(dateRange);

            if (holidayDate.Count == 0)
            {
                return NoContent();
            }

            return Ok(holidayDate);
        }
    }
}