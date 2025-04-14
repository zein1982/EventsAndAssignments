using System.ComponentModel;
using EventsAndAssignments.API.Atributes;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class RemoveRequest<T>
    {
        /// <summary>
        /// Причина удаления
        /// </summary>
        [DefaultValue("Причина удаления")]
        public string? Description { get; set; } = string.Empty;

        /// <summary>
        /// Элементы для удаления
        /// </summary>
        [DefaultValue("[]")]
        [IdListValidation]
        public IReadOnlyCollection<T>? ItemsToRemove { get; set; }
    }
}