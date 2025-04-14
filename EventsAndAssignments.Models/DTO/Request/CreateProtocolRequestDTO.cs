using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class CreateProtocolRequestDTO
    {
        /// <summary>
        /// Дата создания протокола.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Пользователь котоый создал протокол.
        /// </summary>
        [DefaultValue("34cbf4a3-a9e8-ed11-b3d6-0050569a16c1")]
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Идентификатор папки, в которой будет создан протокол.
        /// </summary>
        [DefaultValue(10)]
        [Range(1, long.MaxValue, ErrorMessage = "Недопустимое значение идентификатора")]
        public long FolderId { get; set; }
    }
}