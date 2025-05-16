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
        Console.WriteLine("Route values:");
        foreach (var kvp in context.Request.RouteValues)
        {
            Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
        }
        
        var path = context.Request.Path.Value ?? "";

        // Skip setting tenant context for Identity and public controllers
        var isIdentityArea = context.Request.RouteValues.TryGetValue("area", out var area) &&
                             area?.ToString()?.Equals("Identity", StringComparison.OrdinalIgnoreCase) == true;

        var isPublicController = context.Request.RouteValues.TryGetValue("controller", out var controllerObj) &&
                                 controllerObj is string controller &&
                                 _publicControllers.Contains(controller);

        if (isIdentityArea || isPublicController)
        {
            await next(context);
            return;
        }

        // 1. Check for tenantId in route
        var tenantIdFromRoute = context.Request.RouteValues["tenantId"]?.ToString();
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

        // 2. If logged in, resolve from user and redirect to tenant URL if needed
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

        // 3. Try resolving from invitation
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