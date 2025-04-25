using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace CitizenPanel.UI.MVC.Areas.Identity.UserStores;

public class ApplicationUserStore(PanelDbContext context, Tenant tenant, IdentityErrorDescriber describer = null)
    : UserStore<ApplicationUser>(context, describer)
{
    public override Task<IdentityResult> CreateAsync(ApplicationUser user,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (user is ITenanted tenantedUser)
        {
            tenantedUser.TenantId = tenant.Id;
        }
        
        return base.CreateAsync(user, cancellationToken);
    }
};