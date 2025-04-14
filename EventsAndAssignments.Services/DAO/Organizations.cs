using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EventsAndAssignments.Services.DAO
{
    //Схема по умолчаниюб
    [Table("PuplicOrganizationsView")]
    public class Organization
    {
        [Key]
        public Guid OrganizationId { get; set; }

        public string? DisplayName { get; set; }
        public string? Name { get; set; }
        public string? FullName { get; set; }
        public string? ContatsName { get; set; }
        public string? Kskcode { get; set; }
        public string? Code { get; set; }
        public string? OldOrganizationCode { get; set; }
        public string? UniqueId { get; set; }
        public string? Country { get; set; }
        public string? Region { get; set; }
        public string? RegionCode { get; set; }
        public string? RegionName { get; set; }
        public string? DivisionCode { get; set; }
        public string? DivisionName { get; set; }
        public string? GroupCode { get; set; }
        public string? GrounName { get; set; }
        public string? Root { get; set; }
        public DateTime LastModificationDate { get; set; }
        public string? IsNewMa { get; set; }
        public bool? IsCsrcompany { get; set; }
        public bool? IsHiden { get; set; }
        public ICollection<Assignment>? Assignments { get; set; }
    }
}