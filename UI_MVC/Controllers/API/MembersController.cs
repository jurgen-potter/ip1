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
public class MembersController(IPanelManager panelManager, IUserProfileManager userProfileManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet("{panelId}")]
    public IActionResult Get(int panelId)
    {
        var members = panelManager.GetMembersByPanelId(panelId)
            .OrderByDescending(u => u.IsStaff)
            .ToList();
        
        if (members.Count == 0) {
            return NoContent();
        }
        
        List<MemberProfileDto> memberDtos = [];
        foreach (var member in members)
        {
            var memberDto = new MemberProfileDto()
            {
                Id = member.Id,
                Name = member.MemberProfile.FirstName + " " + member.MemberProfile.LastName,
                Email = member.Email,
                IsStaff = member.IsStaff
            };
            memberDtos.Add(memberDto);
        }
        
        return Ok(memberDtos);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = userProfileManager.GetUserById(id);

        if (user == null)
            return NotFound();

        if (user.UserType != UserType.Member)
            return BadRequest("User is not a member.");

        panelManager.RemoveUserVotesByMember(user);
        
        await userManager.DeleteAsync(user);
    
        return NoContent();
    }
}