using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class UpdateProtocolRequestDTO
    {
        /// <summary>
        /// Идентификатор обновляемого протокола.
        /// </summary>
        [DefaultValue(10)]
        [Range(1, long.MaxValue, ErrorMessage = "Недопустимое значение идентификатора")]
        public long Id { get; set; }

        /// <summary>
        /// Имя нового протокола.
        /// </summary>
        [DefaultValue("Обновленное имя протокола")]
        public string Name { get; set; } = string.Empty;
    }
}