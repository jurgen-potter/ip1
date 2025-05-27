using CitizenPanel.DAL.Data;
using CitizenPanel.UI.MVC.Middleware;

namespace CitizenPanel.UI.MVC.Services;

public class ValidTenantConstraint : IRouteConstraint
{
    /*
    private readonly IServiceProvider _serviceProvider;

    public ValidTenantConstraint(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public bool Match(HttpContext httpContext, IRouter route, string routeKey,
        RouteValueDictionary values, RouteDirection routeDirection)
    {
        if (!values.TryGetValue(routeKey, out var value) || value is not string tenantId)
            return false;

        // Use a scoped service provider to get the DbContext
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PanelDbContext>();

        return db.Tenants.Any(t => t.Id == tenantId);
    }*/
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