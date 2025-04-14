using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments.API.Atributes
{
    public class IdListValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var idList = value as List<long>;

            if (idList is { Count: 0 })
            {
                return new ValidationResult("Список не может быть пустым");
            }

            if (idList.Any(x => x <= 0))
            {
                return new ValidationResult("В списке есть некорректный Id");
            }

            return ValidationResult.Success;
        }
    }
}