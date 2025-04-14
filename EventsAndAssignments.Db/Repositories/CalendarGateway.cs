using EventsAndAssignments.Services.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EventsAndAssignments.Db.Repositories
{
    public class CalendarGateway : ICalendarGateway
    {
        private readonly HseDbContext _context;

        public CalendarGateway(HseDbContext context)
        {
            _context = context;
        }

        public IReadOnlyList<DateOnly> GetHolidayDates(DateTime startDate, DateTime endDate)
        {
            List<DateOnly> result  = _context.HolidayInfo
                .AsNoTracking()
                .Where(e => e.FreeDay && e.Date >= startDate && e.Date <= endDate)
                .Select(e => DateOnly.FromDateTime(e.Date))
                .ToList();

            return result;
        }
    }
}