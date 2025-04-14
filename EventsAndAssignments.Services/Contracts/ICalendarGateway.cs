namespace EventsAndAssignments.Services.Contracts
{
    public interface ICalendarGateway
    {
        public IReadOnlyList<DateOnly> GetHolidayDates(DateTime startDate, DateTime endDate);
    }
}