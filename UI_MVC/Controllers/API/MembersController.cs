using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/{tenantId}/api/[controller]")]
[Authorize]
public class MembersController(IPanelManager panelManager) : ControllerBase
{
    [HttpGet("{id}")]
    [Authorize(Roles = "Organization")]
    public IActionResult Get(int id)
    {
        var panel = panelManager.GetPanelByIdWithMembers(id);
        List<MemberProfileDto> users = new List<MemberProfileDto>();
        
        foreach (var panelMember in panel.Members)
        {
            users.Add(new MemberProfileDto
            {
                Age = DateTime.Today.AddYears(-panelMember.BirthDate.Year).Year,
                Email = panelMember.ApplicationUser.Email,
                Gender = panelMember.Gender.ToDutch()
            });
        }
        return Ok(users);
    }
}