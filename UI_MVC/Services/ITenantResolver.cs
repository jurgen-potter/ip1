using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.UI.MVC.Services;

public interface ITenantResolver
{
    Tenant ResolveTenantFromUser(ApplicationUser user);
    Tenant ResolveTenantFromQuery(HttpContext context);
}