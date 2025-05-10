using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Tenancy;

public interface ITenantManager
{
    Tenant AddTenant(string name);
}