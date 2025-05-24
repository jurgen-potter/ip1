using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.UI.MVC.Services;

public interface ITenantAccessService
{
    Task<bool> IsUserInCurrentTenantAsync();
    Task<string> GetUserTenantIdAsync();
}

public class TenantAccessService : ITenantAccessService
{
    private readonly TenantContext _tenantContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public TenantAccessService(
        TenantContext tenantContext,
        IHttpContextAccessor httpContextAccessor,
        UserManager<ApplicationUser> userManager)
    {
        _tenantContext = tenantContext;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<bool> IsUserInCurrentTenantAsync()
    {
        var currentTenantId = _tenantContext.Tenant?.Id;
        var userTenantId = await GetUserTenantIdAsync();

        return currentTenantId != null && userTenantId != null && currentTenantId == userTenantId;
    }

    public async Task<string> GetUserTenantIdAsync()
    {
        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);
        return user?.UserType switch
        {
            UserType.Member => user.MemberProfile?.TenantId,
            UserType.Organization => user.OrganizationProfile?.TenantId,
            _ => null
        };
    }
}
