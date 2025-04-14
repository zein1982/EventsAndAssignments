using EventsAndAssignments.API.Atributes;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class ByIdListRequest
    {
        [IdListValidation]
        public List<long> IdList { get; set; } = new();
    }
}