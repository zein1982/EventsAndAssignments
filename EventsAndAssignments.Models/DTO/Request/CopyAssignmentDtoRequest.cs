namespace EventsAndAssignments.Models.DTO.Request
{
    /// <summary>
    /// DTO которая будет пердаваться в контроллер копирования поручения
    /// </summary>
    public class CopyAssignmentDtoRequest
    {
        /// <summary>
        /// Id копируемого поручения
        /// </summary>
        public ICollection<long>? AssignmentsIds { get; set; }

        /// <summary>
        /// Id компании для которой назначено это поручение
        /// </summary>
        public long ProtocolId { get; set; }
    }
}