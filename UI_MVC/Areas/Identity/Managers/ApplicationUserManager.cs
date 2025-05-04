using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
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
        TenantContext tenantContext
    ) : base(store, optionsAccessor, passwordHasher, userValidators,
        passwordValidators, keyNormalizer, errors, services, logger)
    {
        _context = context;
        _tenantContext = tenantContext;
    }
    
    public async Task<IdentityResult> CreateWithTenantAsync(ApplicationUser user, string password, string givenTenantId = null, bool generateNewTenantId = false)
    {
        if (user.UserType == UserType.Member && user.MemberProfile != null)
        {
            user.MemberProfile.TenantId = givenTenantId ?? (generateNewTenantId ? Guid.NewGuid().ToString() : _tenantContext.GetCurrentTenantId());
        }
        else if (user.UserType == UserType.Organization && user.OrganizationProfile != null)
        {
            user.OrganizationProfile.TenantId = givenTenantId ?? (generateNewTenantId ? Guid.NewGuid().ToString() : _tenantContext.GetCurrentTenantId());
        }

        return await CreateAsync(user, password);
    }
    
    private IQueryable<ApplicationUser> GetUserWithProfilesBase()
    {
        return _context.Users
            .Include(u => u.MemberProfile)
            .Include(u => u.OrganizationProfile)
            .IgnoreQueryFilters();
    }

    public async Task<ApplicationUser> GetUserWithProfilesByIdAsync(string userId)
    {
        var user = await GetUserWithProfilesBase()
            .SingleOrDefaultAsync(u => u.Id == userId);
        return user;
    }
    
    public async Task<ApplicationUser> GetUserWithProfilesAndPanelsAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = GetUserId(userPrincipal);
        var user = await GetUserWithProfilesBase()
            .Include(u => u.MemberProfile.Panels)
            .SingleOrDefaultAsync(u => u.Id == userId);
        return user;
    }
    
    public async Task<ApplicationUser> GetUserWithProfilesAsync(ClaimsPrincipal userPrincipal)
    {
        var userId = GetUserId(userPrincipal);
        var user = await GetUserWithProfilesByIdAsync(userId);
        return user;
    }
}