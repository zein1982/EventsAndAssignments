using System.ComponentModel.DataAnnotations.Schema;
using EventsAndAssignments.Services.Extensions;

namespace EventsAndAssignments.Services.DAO
{
    [Table("PuplicEmployeeViews")]
    public class Employee
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
        public Guid PositionId { get; set; }
        public string? PositionCode { get; set; }
        public string? PositionName { get; set; }
        public string? DepartmentCode { get; set; }
        public string? DepartmentName { get; set; }
        // TODO Убрать загрузку фото?
        public byte[]? Photo { get; set; }
        public byte[]? PhotoS { get; set; }
        public string? IsSfrelevant { get; set; }
        public string? Occupation { get; set; }
        public long? RoleId { get; set; }

        [Column("PersonLastModfication")]
        public DateTime PersonLastModification { get; set; }

        [Column("PositionLastModfication")]
        public DateTime PositionLastModification { get; set; }

        [Column("DepartmentLastModfication")]
        public DateTime DepartmentLastModification { get; set; }

        [Column("OrganizationLastModfication")]
        public DateTime OrganizationLastModification { get; set; }

        public string? HireDate { get; set; }
        public string? EndDate { get; set; }

        [Column("AnyLastModfication")]
        public DateTime AnyLastModification { get; set; }

        public bool IsActive {  get; set; }

        //Навигационные свойства
        public Role? UserRole { get; set; }
        public virtual ICollection<ProtocolFolder>? ProtocolFoldersCreatedByNavigation { get; set; }
        public virtual ICollection<ProtocolFolder>? ProtocolFoldersUpdatedByNavigation { get; set; }
        public virtual ICollection<ProtocolFolder>? ProtocolFoldersAllowedEmployeesNavigation { get; set; }
        public virtual ICollection<Protocol>? ProtocolsCreatedByNavigation { get; set; }
        public virtual ICollection<Protocol>? ProtocolsUpdatedByNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsCreatedByNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsUpdatedByNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsAuthorNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsResponsibleExecutorNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsResponsibleLeaderNavigation { get; set; }
        public virtual ICollection<Assignment>? AssignmentsResponsibleInspectorNavigation { get; set; }
        public virtual ICollection<AssignmentHistory>? AssignmentHistoryCreatedByNavigation { get; set; }
        public virtual ICollection<AssignmentHistory>? AssignmentHistoryAddedResponsibleExecutorNavigation { get; set; }
        public virtual ICollection<AssignmentHistory>? AssignmentHistoryRemovedResponsibleExecutorNavigation { get; set; }
        public virtual ICollection<AssignmentFile>? AssignmentFilesCreatedByNavigation { get; set; }
        public virtual ICollection<AssignmentFile>? AssignmentFilesUpdatedByNavigation { get; set; }
        public virtual ICollection<Comment>? CommentsCreatedByNavigation { get; set; }
        public virtual ICollection<Comment>? CommentsUpdatedByNavigation { get; set; }
        public virtual ICollection<NotificationSetting>? NotificationSettingUserNavigation { get; set; }

        /// <summary>
        /// Получени имени фамилии и отчества пользователя.
        /// </summary>
        public string GetFullName() => $"{LastName} {FirstName} {MiddleName}";

        /// <summary>
        /// Получаем фамилию с инициалами.
        /// </summary>
        public string GetInitials() => $"{LastName!.ToLower()} {FirstName![0]}.{MiddleName![0]}.".CapitalizeWords();

        public string GetFormatedName() =>
            $"{LastName} {FirstName} {MiddleName}".ToLower().CapitalizeWords();
    }
}