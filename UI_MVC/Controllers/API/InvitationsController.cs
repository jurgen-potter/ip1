using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.Draws;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/{tenantId}/api/[controller]")]
[Authorize(Roles = "Organization")]
public class InvitationsController(IPanelManager panelManager) : ControllerBase
{
    [HttpGet("{panelId}")]
    public IActionResult Get(int panelId)
    {
        var reserves = panelManager.GetReservesByPanelId(panelId).ToList();
        
        if (reserves.Count == 0) {
            return NoContent();
        }
        
        List<InvitationDto> invitationDtos = [];
        foreach (var reserve in reserves)
        {
            var invitationDto = new InvitationDto()
            {
                Id = reserve.Id,
                Email = reserve.Email
            };
            invitationDtos.Add(invitationDto);
        }
        
        return Ok(invitationDtos);
    }
    
    [HttpPost("makeInvitations/{panelId}")]
    public IActionResult MakeInvitations([FromBody] NewInvitationsDto model)
    {
        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(model.PanelId).ToList();
        var panel = panelManager.GetPanelById(model.PanelId);
        
        var buckets = model.Buckets.Where(b => !b.IsSufficient);

        var missingMembers = 0;
        foreach (var bucket in buckets)
        {
            missingMembers += bucket.Count - bucket.RegisteredCount;
        }
        var result = utilityManager.CalculateRecruitment(missingMembers, criteria);
        
        
        var invitations = utilityManager.GenerateInvitations(result.TotalNeededInvitations, criteria, panel);
        panel.Invitations = invitations.ToList();
        panelManager.EditPanel(panel);
        return Ok();
    }
}