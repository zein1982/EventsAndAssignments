using EventsAndAssignments.Models.Primitives;

namespace EventsAndAssignments.Services.DAO
{
    public sealed class Role : Enumeration<Role>
    {
        public static readonly Role SystemAdmin = new(1, "SystemAdmin");
        public static readonly Role Admin = new(2, "Admin");
        public static readonly Role SimpleUser = new(3, "SimpleUser");

        public Role(long id, string name)
            : base(id, name)
        {
        }

        public ICollection<Permission> Permissions { get; set; }
        public ICollection<Employee> Employees { get; set; }
    }
}