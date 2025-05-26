using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/{tenantId}/api/[controller]")]
[Authorize(Roles = "Organization")]
public class OrganizationsController(IUserProfileManager userProfileManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var currentUser = userManager.GetUserAsync(User).Result;
        if (currentUser is { IsSuper: false })
        {
            return Forbid("Alleen root organisatie beheerders kunnen organisatie accounts verwijderen.");
        }
        
        var staffOrgs = userProfileManager.GetAllOrganizations()
            .Where(u => u.Id != currentUser?.Id)
            .OrderByDescending(o => o.IsSuper)
            .ToList();
        
        if (staffOrgs.Count == 0) {
            return NoContent();
        }
        
        List<OrganizationDto> organizationDtos = [];
        foreach (var staff in staffOrgs)
        {
            var organizationDto = new OrganizationDto()
            {
                Id = staff.Id,
                Email = staff.Email,
                IsSuper = staff.IsSuper
            };
            organizationDtos.Add(organizationDto);
        }
        
        return Ok(organizationDtos);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is { IsSuper: false })
        {
            return Forbid("Alleen super organisatie beheerders kunnen organisatie accounts verwijderen.");
        }
        
        var user = userProfileManager.GetUserById(id);

        if (user == null)
            return NotFound();

        if (user.UserType != UserType.Organization)
            return BadRequest("User is not an organization account.");
        
        await userManager.DeleteAsync(user);
    
        return NoContent();
    }
}