using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using System.Security.Claims;

namespace CitizenPanel.UI.MVC.Middleware;

public class TenantMiddleware : IMiddleware
{
    private readonly TenantContext _tenantContext;
    private readonly TenantUserManager _userManager;

    public TenantMiddleware(TenantContext tenantContext, TenantUserManager userManager)
    {
        _tenantContext = tenantContext;
        _userManager = userManager;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _userManager.GetUserWithProfilesAsync(userId);
                
                string tenantId = null;
                
                if (user?.UserType == UserType.Member && user.MemberProfile != null)
                {
                    tenantId = user.MemberProfile.TenantId;
                }
                else if (user?.UserType == UserType.Organization && user.OrganizationProfile != null)
                {
                    tenantId = user.OrganizationProfile.TenantId;
                }
                
                if (!string.IsNullOrEmpty(tenantId))
                {
                    _tenantContext.Tenant = new Tenant { Id = tenantId };
                }
            }
        }
        
        await next(context);
    }
}