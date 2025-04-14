using EventsAndAssignments.Services.Contracts;
using EventsAndAssignments.Services.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EventsAndAssignments.Db.Repositories
{
    public class OrganizationGateway : IOrganizationGateway
    {
        private readonly ApplicationDbContext _context;

        public OrganizationGateway(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Organization> CreateOrganization(Organization organization)
        {
            _context.Organizations.Add(organization);
            await _context.SaveChangesAsync();
            return organization;
        }

        public async Task<IReadOnlyCollection<Organization>> GetOrganizations(string? name)
        {
            return name.IsNullOrEmpty()
                ? await _context.Organizations.ToListAsync()
                : await _context.Organizations.Where(c => c.Name!.Contains(name!)).ToListAsync();
        }

        public async Task<Organization> GetOrganizationByIdAsync(Guid id)
        {
            Organization? organization = await _context.Organizations.FindAsync(id);
            return organization;
        }
    }
}