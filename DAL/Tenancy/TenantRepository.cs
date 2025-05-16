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
}