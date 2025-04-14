using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.Models.DTO.Request
{
    public class CreateFolderRequest
    {
        /// <summary>
        /// Наименование новой папки.
        /// </summary>
        [DefaultValue("Новая папка")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Наименование папки обязательно")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Список администраторов папки
        /// </summary>
        public ICollection<Guid> AllowedEmployeesIds { get; set; } = new List<Guid>();
    }
}