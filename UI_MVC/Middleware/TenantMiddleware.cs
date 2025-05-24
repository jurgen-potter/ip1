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
    
    private readonly HashSet<(string Controller, string Action)> _tenantSpecificRoutes = new()
    {
        ("panel", "details")
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var path = context.Request.Path.Value ?? "";
        var area = context.Request.RouteValues.TryGetValue("area", out var areaObj) ? areaObj?.ToString() : null;
        var controller = context.Request.RouteValues.TryGetValue("controller", out var controllerObj) ? controllerObj?.ToString()?.ToLower() : null;
        var action = context.Request.RouteValues.TryGetValue("action", out var actionObj) ? actionObj?.ToString()?.ToLower() : null;

        // Skip setting tenant context for Identity and public controllers
        var isIdentityArea = area?.Equals("Identity", StringComparison.OrdinalIgnoreCase) == true;
        var isPublicController = _publicControllers.Contains(controller);
        var isTenantSpecificRoute = controller != null && action != null && _tenantSpecificRoutes.Contains((controller, action));

        if (isIdentityArea || isPublicController)
        {
            // Remove tenantId from URL path if it exists
            var routeTenantId = context.Request.RouteValues["tenantId"]?.ToString();
            if (!string.IsNullOrEmpty(routeTenantId) && path.StartsWith($"/{routeTenantId}", StringComparison.OrdinalIgnoreCase))
            {
                var newPath = path[$"/{routeTenantId}".Length..]; // remove tenantId from beginning
                if (string.IsNullOrWhiteSpace(newPath))
                    newPath = "/"; // default to root if empty

                var query = context.Request.QueryString;
                context.Response.Redirect($"{newPath}{query}");
                return;
            }
            
            await next(context);
            return;
        }

        var tenantIdFromRoute = context.Request.RouteValues["tenantId"]?.ToString();
        if (!isTenantSpecificRoute)
        {
            // Resolve from logged in user
            if (!string.IsNullOrEmpty(tenantIdFromRoute))
            {
                path = path[$"/{tenantIdFromRoute}".Length..];
                if (string.IsNullOrEmpty(path))
                    path = "/";
            
                context.Request.Path = new PathString(path);
            }
            
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var user = userProfileManager.GetUserByIdWithProfile(userId);
                    var tenant = tenantResolver.ResolveTenantFromUser(user);

                    if (tenant is not null)
                    {
                        tenantContext.Tenant = tenant;

                        if (!path.StartsWith($"/{tenant.Id}", StringComparison.OrdinalIgnoreCase))
                        {
                            var newPath = $"/{tenant.Id}{path}{context.Request.QueryString}";
                            context.Response.Redirect(newPath);
                            return;
                        }

                        await next(context);
                        return;
                    }
                }
            }
        }
        
        // Check for tenantId in route
        if (!string.IsNullOrEmpty(tenantIdFromRoute))
        {
            var tenant = tenantManager.GetTenantById(tenantIdFromRoute);
            if (tenant == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            tenantContext.Tenant = tenant;
            await next(context);
            return;
        }

        // Resolve from invitation
        var invitationTenant = tenantResolver.ResolveTenantFromQuery(context);
        if (invitationTenant is not null)
        {
            if (!path.StartsWith($"/{invitationTenant.Id}", StringComparison.OrdinalIgnoreCase))
            {
                var newPath = $"/{invitationTenant.Id}{path}{context.Request.QueryString}";
                context.Response.Redirect(newPath);
                return;
            }
        }

        await next(context);
    }
}