using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class UpdateFolderRequest
    {
        /// <summary>
        /// Идентификатор папки для обновления.
        /// </summary>
        [Required]
        [Range(1, long.MaxValue, ErrorMessage = "недопустимое значение идентификатора")]
        public long Id { get; set; }

        /// <summary>
        /// Новое название папки
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Идентификатор создателя папки
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        /// Список администраторов папки
        /// </summary>
        public ICollection<Guid> AllowedEmployeesIds { get; set; } = new List<Guid>();
    }
}