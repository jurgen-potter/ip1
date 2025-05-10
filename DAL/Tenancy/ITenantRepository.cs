using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.DAL.Tenancy;

public interface ITenantRepository
{
    void CreateTenant(Tenant tenant);
    bool TenantIdExists(string tenantId);
}