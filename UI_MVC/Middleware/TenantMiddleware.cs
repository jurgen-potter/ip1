using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using System.Security.Claims;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Services;

namespace CitizenPanel.UI.MVC.Middleware;

public class TenantMiddleware(
    TenantContext tenantContext,
    ApplicationUserManager userManager,
    IDrawManager drawManager,
    IUserProfileManager userProfileManager,
    ITenantResolver tenantResolver) : IMiddleware
{
    // List of controllers that don't require a tenant context
    private readonly HashSet<string> _publicControllers = new(StringComparer.OrdinalIgnoreCase)
    {
        "Home"
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.RouteValues.TryGetValue("controller", out var controllerObj) && controllerObj is string controller && _publicControllers.Contains(controller)
            || context.Request.RouteValues.TryGetValue("area", out var area) && area?.ToString()?.Equals("Identity", StringComparison.OrdinalIgnoreCase) == true)
        {
            // For public controllers, leave tenant context empty
            await next(context);
            return;
        }
        
        // First try to get tenant from route data
        string tenantIdFromRoute = context.Request.RouteValues["tenantId"]?.ToString();
        
        if (!string.IsNullOrEmpty(tenantIdFromRoute))
        {
            tenantContext.Tenant = new Tenant { Id = tenantIdFromRoute };
            await next(context);
            return;
        }
        
        // If no tenant in route, try to get from authenticated user
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = userProfileManager.GetUserByIdWithProfile(userId);
                string tenantId = tenantResolver.ResolveTenantFromUser(user);
                
                if (!string.IsNullOrEmpty(tenantId))
                {
                    if (context.Request.Path.Value != null && !context.Request.Path.Value.StartsWith($"/{tenantId}", StringComparison.OrdinalIgnoreCase))
                    {
                        var newPath = $"/{tenantId}{context.Request.Path}{context.Request.QueryString}";
                        context.Response.Redirect(newPath, permanent: false);
                        return;
                    }

                    await next(context);
                    return;
                }
            }
        }
        
        // As a last resort, try to get from invitation
        string tenantIdFromInvitation = tenantResolver.ResolveTenantFromQuery(context);
        
        if (!string.IsNullOrEmpty(tenantIdFromInvitation))
        {
            if (context.Request.Path.Value != null && !context.Request.Path.Value.StartsWith($"/{tenantIdFromInvitation}", StringComparison.OrdinalIgnoreCase))
            {
                var newPath = $"/{tenantIdFromInvitation}{context.Request.Path}{context.Request.QueryString}";
                context.Response.Redirect(newPath, permanent: false);
                return;
            }
        }
        
        await next(context);
    }
}