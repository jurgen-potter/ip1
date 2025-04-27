using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CitizenPanel.BL.Domain.Tenancy;

public class Tenant
{
    public string Id { get; set; }
}

public class AvailableTenants
{
    public const string SectionName = nameof(AvailableTenants);
    public Tenant[] Tenants { get; set; }
}

public class TenantContext
{
    public Tenant Tenant { get; set; } = new();
    public string GetCurrentTenantId() => Tenant?.Id ?? string.Empty;
}

public static class TenantExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<TenantContext>();
        services.AddTransient<Tenant>(p => p.GetRequiredService<TenantContext>().Tenant);
        return services;
    }
}