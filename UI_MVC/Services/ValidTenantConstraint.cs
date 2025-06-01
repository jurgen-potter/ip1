using CitizenPanel.DAL.Data;
using CitizenPanel.UI.MVC.Middleware;

namespace CitizenPanel.UI.MVC.Services;

public class ValidTenantConstraint : IRouteConstraint
{
    private readonly TenantStore _tenantStore;
    private readonly IServiceProvider _serviceProvider;

    public ValidTenantConstraint(TenantStore tenantStore, IServiceProvider serviceProvider)
    {
        _tenantStore = tenantStore;
        _serviceProvider = serviceProvider;
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is not string tenantId)
            return false;

        if (_tenantStore.TenantIds.Contains(tenantId))
            return true;

        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PanelDbContext>();

        var exists = db.Tenants.Any(t => t.Id == tenantId);
        if (exists)
        {
            _tenantStore.TenantIds.Add(tenantId);
        }

        return exists;
    }
}