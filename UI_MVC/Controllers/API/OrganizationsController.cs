using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/{tenantId}/api/[controller]")]
[Authorize(Roles = "Organization")]
public class OrganizationsController(IUserProfileManager userProfileManager, UserManager<ApplicationUser> userManager, IPanelManager panelManager) : ControllerBase
{
    [HttpGet]
    public IActionResult GetOrganizationAccounts()
    {
        var currentUser = userManager.GetUserAsync(User).Result;
        if (currentUser is { IsSuper: false }) { return Forbid(); }

        var staffOrgs = userProfileManager.GetAllOrganizations()
            .Where(u => u.Id != currentUser?.Id)
            .OrderByDescending(o => o.IsSuper)
            .ToList();

        if (staffOrgs.Count == 0)
        {
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

    [HttpGet("{panelId}")]
    public IActionResult GetOrganizationMembers(int panelId)
    {
        var staffOrgs = userProfileManager.GetAllOrganizationMembersNotInPanel(panelId).ToList();

        if (staffOrgs.Count == 0) { return NoContent(); }

        List<MemberProfileDto> organizationMemberDtos = [];
        foreach (var staff in staffOrgs)
        {
            var organizationMemberDto = new MemberProfileDto()
            {
                Id = staff.ApplicationUser.Id,
                Name = staff.FirstName + " " + staff.LastName
            };
            organizationMemberDtos.Add(organizationMemberDto);
        }

        return Ok(organizationMemberDtos);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is { IsSuper: false }) { return Forbid("Alleen super organisatie beheerders kunnen organisatie accounts verwijderen."); }

        var user = userProfileManager.GetUserById(id);

        if (user == null)
            return NotFound();

        if (user.UserType != UserType.Organization)
            return BadRequest("User is not an organization account.");

        await userManager.DeleteAsync(user);

        return NoContent();
    }

    [HttpPost("{panelId}/add-user/{userId}")]
    public IActionResult AddUserToPanel(int panelId, string userId)
    {
        var panel = panelManager.GetPanelByIdWithMembers(panelId);

        if (panel == null)
            return NotFound();

        var user = userProfileManager.GetUserByIdWithProfile(userId);

        if (user == null)
            return NotFound();

        if (panel.Members.Any(u => u.ApplicationUser.Id == userId) || user.UserType != UserType.Member)
            return BadRequest();

        panel.Members.Add(user.MemberProfile);
        panelManager.EditPanel(panel);

        return Ok();
    }
}