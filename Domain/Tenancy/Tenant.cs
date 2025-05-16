using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Tenancy;

public class Tenant
{
    public string Id { get; set; }
    
    [MaxLength(25, ErrorMessage = "Organisatienaam mag niet langer zijn dan 25 tekens.")]
    public string Name { get; set; }
}

public class TenantContext
{
    public Tenant Tenant { get; set; } = new();
}

public static class TenantExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<TenantContext>();
        return services;
    }
}