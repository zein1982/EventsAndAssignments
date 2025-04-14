using EventsAndAssignments.Models.DTO.Request;
using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.Interfaces;

namespace EventsAndAssignments.Services.Data
{
    public class CalendarService : ICalendarService
    {
        private readonly ICalendarGateway _calendarGateway;

        public CalendarService(ICalendarGateway calendarGateway)
        {
            _calendarGateway = calendarGateway;
        }

        public IReadOnlyList<DateOnly> GetHolidayDates(DateRangeRequestDto dateRange)
        {
            IReadOnlyList<DateOnly> holidays = _calendarGateway
                    .GetHolidayDates(dateRange.StartDate, dateRange.EndDate);

            return holidays;
        }
    }
}