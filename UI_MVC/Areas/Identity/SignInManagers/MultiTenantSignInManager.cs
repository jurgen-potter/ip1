using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CitizenPanel.UI.MVC.Areas.Identity.SignInManagers;

public class MultiTenantSigninManager(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor contextAccessor,
    IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
    IOptions<IdentityOptions> optionsAccessor,
    ILogger<SignInManager<ApplicationUser>> logger,
    IAuthenticationSchemeProvider schemes,
    IUserConfirmation<ApplicationUser> confirmation,
    Tenant tenant
)
    : SignInManager<ApplicationUser>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes,
        confirmation)
{
    public override Task<SignInResult> PasswordSignInAsync(ApplicationUser user, string password, bool isPersistent,
        bool lockoutOnFailure)
    {
        if (user is ITenanted tenantedUser)
        {
            if (tenantedUser.TenantId != tenant.Id)
            {
                return Task.FromResult(SignInResult.Failed);
            }
        }

        return base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);
    }
};