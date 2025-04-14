using EventsAndAssignments.Models.DTO.Request;

namespace EventsAndAssignments.Services.Interfaces
{
    public interface ICalendarService
    {
        public IReadOnlyList<DateOnly> GetHolidayDates(DateRangeRequestDto dateRange);
    }
}