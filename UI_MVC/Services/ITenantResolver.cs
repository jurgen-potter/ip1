using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.UI.MVC.Services;

public interface ITenantResolver
{
    string ResolveTenantFromUser(ApplicationUser user);
    string ResolveTenantFromQuery(HttpContext context);
}