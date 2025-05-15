using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.UI.MVC.Models;
using System.ComponentModel.DataAnnotations;
using CitizenPanel.UI.MVC.Models.Draws;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CitizenPanel.UI.MVC.Validation;

public class UniqueCriteriaAttribute : ValidationAttribute
{
    public UniqueCriteriaAttribute()
    {
        ErrorMessage = "Duplicate criteria names are not allowed.";
    }

    public override bool IsValid(object? value)
    {
        if (value is List<CriteriaViewModel> criteriaList)
        {
            var duplicates = criteriaList
                .GroupBy(c => c.Name?.ToLowerInvariant())
                .Where(g => !string.IsNullOrEmpty(g.Key) && g.Count() > 1)
                .Select(g => g.Key)
                .ToList();
            
            if (duplicates.Any())
            {
                ErrorMessage = $"De volgende criteria komen meerdere keren voor: {string.Join(", ", duplicates)}";
                return false;
            }
            
            return true;
        }

        return false;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireTenantAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var tenantContext = context.HttpContext.RequestServices.GetService<TenantContext>();
        
        // If no tenant is found in the context, redirect to home page
        if (tenantContext == null || string.IsNullOrEmpty(tenantContext.Tenant.Id))
        {
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}