using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.DAL.Tenancy;

namespace CitizenPanel.BL.Tenancy;

public class TenantManager(ITenantRepository repository) : ITenantManager
{
    public Tenant AddTenant(string name)
    {
        string baseId = name.Trim().ToLower().Replace(' ', '-');
        string tenantId = baseId;
        int counter = 1;

        while (repository.TenantIdExists(tenantId))
        {
            tenantId = $"{baseId}{counter}";
            counter++;
        }
        
        Tenant tenant = new Tenant()
        {
            Id = tenantId,
            Name = name
        };
        repository.CreateTenant(tenant);
        return tenant;
    }
}