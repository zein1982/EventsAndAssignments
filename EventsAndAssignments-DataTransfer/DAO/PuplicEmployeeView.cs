using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventsAndAssignments_DataTransfer.DAO
{
    public partial class PuplicEmployeeView
    {
        public Guid EmployeeId { get; set; }
        public string? TabelNumber { get; set; }
        public string? Domain { get; set; }
        public string? Login { get; set; }
        public string? Email { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? OrganizationCode { get; set; }
        public string? OrganizationName { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid PositionId { get; set; }

        public string? PositionCode { get; set; }
        public string? PositionName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        // HACK !! Загрузка фотографий реализована отдельной вне этой структуры данных
        //public byte[]? Photo { get; set; }
        //public byte[]? PhotoS { get; set; }
        public string? IsSfrelevant { get; set; }
        public string? Occupation { get; set; }
        public DateTime PersonLastModfication { get; set; }
        public DateTime PositionLastModfication { get; set; }
        public DateTime DepartmentLastModfication { get; set; }
        public DateTime OrganizationLastModfication { get; set; }
        public string? HireDate { get; set; }
        public string? EndDate { get; set; }
        public DateTime AnyLastModfication { get; set; }
    }
}
