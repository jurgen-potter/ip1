using Microsoft.Extensions.DependencyInjection;

namespace CitizenPanel.BL.Domain.Tenancy;

public class Tenant
{
    public string Id { get; set; }
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