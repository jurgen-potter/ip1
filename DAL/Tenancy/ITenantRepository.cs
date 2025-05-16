using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.DAL.Tenancy;

public interface ITenantRepository
{
    void CreateTenant(Tenant tenant);
    Tenant ReadTenantById(string id);
    bool TenantIdExists(string tenantId);
}