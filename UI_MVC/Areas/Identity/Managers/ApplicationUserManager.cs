using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Tenancy;
using CitizenPanel.DAL.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace CitizenPanel.UI.MVC.Areas.Identity.Managers;

public class ApplicationUserManager : UserManager<ApplicationUser>
{
    private readonly PanelDbContext _context;
    private readonly TenantContext _tenantContext;
    private readonly ITenantManager _tenantManager;

    public ApplicationUserManager(
        IUserStore<ApplicationUser> store,
        IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<ApplicationUser> passwordHasher,
        IEnumerable<IUserValidator<ApplicationUser>> userValidators,
        IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
        ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors,
        IServiceProvider services,
        ILogger<UserManager<ApplicationUser>> logger,
        PanelDbContext context,
        TenantContext tenantContext,
        ITenantManager tenantManager
    ) : base(store, optionsAccessor, passwordHasher, userValidators,
        passwordValidators, keyNormalizer, errors, services, logger)
    {
        _context = context;
        _tenantContext = tenantContext;
        _tenantManager = tenantManager;
    }
    
    public async Task<IdentityResult> CreateWithTenantAsync(ApplicationUser user, string password, string givenTenantId = null, string newTenantName = null)
    {
        if (user.UserType == UserType.Member && user.MemberProfile != null)
        {
            user.MemberProfile.TenantId = givenTenantId ?? _tenantContext.Tenant.Id;
        }
        else if (user.UserType == UserType.Organization && user.OrganizationProfile != null)
        {
            if (newTenantName is null)
            {
                user.OrganizationProfile.TenantId = givenTenantId ?? _tenantContext.Tenant.Id;
            }
            else
            {
                Tenant tenant = _tenantManager.AddTenant(newTenantName);
                user.OrganizationProfile.TenantId = tenant.Id;
            }
        }

        return await CreateAsync(user, password);
    }
}