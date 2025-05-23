using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL.Tenancy;

public class TenantRepository(PanelDbContext dbContext) : ITenantRepository
{
    public void CreateTenant(Tenant tenant)
    {
        dbContext.Tenants.Add(tenant);
        dbContext.SaveChanges();
    }

    public Tenant ReadTenantById(string tenantId)
    {
        return dbContext.Tenants.SingleOrDefault(t => t.Id == tenantId);
    }

    public bool TenantIdExists(string tenantId)
    {
        return dbContext.Tenants.Any(t => t.Id == tenantId);
    }

    public ICollection<Tenant> ReadAllTenants()
    {
        return dbContext.Tenants.ToList();
    }
    public bool DeleteTenant(Tenant tenant)
    {
        var userVotes = dbContext.UserVotes
            .Where(uv => uv.TenantId == tenant.Id)
            .ToList();
        
        var memberUsers = dbContext.MemberProfiles
            .Where(mp => mp.TenantId == tenant.Id)
            .Select(mp => mp.ApplicationUser)
            .ToList();

        var organizationUsers = dbContext.OrganizationProfiles
            .Where(op => op.TenantId == tenant.Id)
            .Select(op => op.ApplicationUser)
            .ToList();

        var allUsersToDelete = memberUsers
            .Concat(organizationUsers)
            .ToList();

        dbContext.UserVotes.RemoveRange(userVotes);
        dbContext.Users.RemoveRange(allUsersToDelete);
        
        dbContext.Tenants.Remove(tenant);
        return dbContext.SaveChanges() > 0;
    }
}