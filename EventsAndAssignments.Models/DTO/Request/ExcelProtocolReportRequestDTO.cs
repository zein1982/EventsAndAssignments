using EventsAndAssignments.API.Atributes;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class ExcelProtocolReportRequestDTO
    {
        [IdListValidation]
        public ICollection<long>? Ids { get; set; }

        public int TimeDifference { get; set; }
    }
}