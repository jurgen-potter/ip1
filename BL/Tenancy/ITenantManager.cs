using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Tenancy;

public interface ITenantManager
{
    Tenant AddTenant(string name);
    Tenant GetTenantById(string name);
    ICollection<Tenant> GetAllTenants();
    bool RemoveTenant(Tenant tenant);
    bool TenantExists(string tenantId);
}