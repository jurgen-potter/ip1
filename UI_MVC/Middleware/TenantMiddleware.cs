using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using System.Security.Claims;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Tenancy;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Services;

namespace CitizenPanel.UI.MVC.Middleware;

public class TenantMiddleware(
    TenantContext tenantContext,
    IUserProfileManager userProfileManager,
    ITenantResolver tenantResolver,
    ITenantManager tenantManager) : IMiddleware
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
            var tenant = tenantManager.GetTenantById(tenantIdFromRoute);
            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
            }
            else
            {
                tenantContext.Tenant = tenant;
                await next(context);
            }
            return;
        }
        
        // If no tenant in route, try to get from authenticated user
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = userProfileManager.GetUserByIdWithProfile(userId);
                var tenant = tenantResolver.ResolveTenantFromUser(user);
                
                if (tenant is not null)
                {
                    if (context.Request.Path.Value != null && !context.Request.Path.Value.StartsWith($"/{tenant.Id}", StringComparison.OrdinalIgnoreCase))
                    {
                        var newPath = $"/{tenant.Id}{context.Request.Path}{context.Request.QueryString}";
                        context.Response.Redirect(newPath, permanent: false);
                        return;
                    }

                    await next(context);
                    return;
                }
            }
        }
        
        // As a last resort, try to get from invitation
        var tenantFromInvitation = tenantResolver.ResolveTenantFromQuery(context);
        
        if (tenantFromInvitation is not null)
        {
            if (context.Request.Path.Value != null && !context.Request.Path.Value.StartsWith($"/{tenantFromInvitation}", StringComparison.OrdinalIgnoreCase))
            {
                var newPath = $"/{tenantFromInvitation}{context.Request.Path}{context.Request.QueryString}";
                context.Response.Redirect(newPath, permanent: false);
                return;
            }
        }
        
        await next(context);
    }
}