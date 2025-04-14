namespace EventsAndAssignments.Services.DAO
{
    public class Permission
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Role>? Roles { get; set; }
    }
}