using CitizenPanel.DAL.Data;
using CitizenPanel.UI.MVC.Middleware;

namespace CitizenPanel.UI.MVC.Services;

public class ValidTenantConstraint : IRouteConstraint
{
    private readonly TenantStore _tenantStore;

    public ValidTenantConstraint(TenantStore tenantStore)
    {
        _tenantStore = tenantStore;
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        return values.TryGetValue(routeKey, out var value) && value is string tenantId
                                                           && _tenantStore.TenantIds.Contains(tenantId);
    }

}