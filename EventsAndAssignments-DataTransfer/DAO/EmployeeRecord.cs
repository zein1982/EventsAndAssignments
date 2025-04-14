namespace EventsAndAssignments_DataTransfer.DAO
{
    /// <summary>
    /// Структура сведений из записи о трудозанятом. Предполагается, как
    /// тип, удобный для сравнения сведений между записями о трудозанятых
    /// </summary>
    public record EmployeeRecord
    {
        public Guid EmployeeId { get; set; }

        public Guid PositionId { get; set; }

        public string? TabelNumber {  get; set; }

        public string? Domain { get; set; }

        public string? Login { get; set; }

        public string? Email { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? OrganizationCode { get; set; }

        public string? OrganizationName { get; set; }

        public string? PositionCode { get; set; }

        public string? PositionName { get; set; }

        public string? DepartmentCode { get; set; }

        public string? DepartmentName { get; set; }

        public string? Occupation { get; set; }

        public string? EndDate { get; set; }
    }
}
