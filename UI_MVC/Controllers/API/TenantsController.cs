using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Tenancy;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/api/[controller]")]
public class TenantsController(ITenantManager tenantManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var tenants = tenantManager.GetAllTenants();
        
        if (tenants.Count == 0) {
            return NoContent();
        }
        
        List<TenantDto> tenantDtos = [];
        foreach (var tenant in tenants)
        {
            var tenantDto = new TenantDto()
            {
                Id = tenant.Id,
                Name = tenant.Name
            };
            tenantDtos.Add(tenantDto);
        }
        
        return Ok(tenantDtos);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var tenant = tenantManager.GetTenantById(id);

        if (tenant == null)
            return NotFound();
        
        tenantManager.RemoveTenant(tenant);
        
        return NoContent();
    }
}