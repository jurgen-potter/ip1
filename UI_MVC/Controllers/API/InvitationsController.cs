using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("{tenantId}/api/[controller]")]
[Authorize]
public class InvitationsController(
    IUtilityManager utilityManager,
    IPanelManager panelManager) : ControllerBase
{
    [HttpPost("makeInvitations/{panelId}")]
    public IActionResult MakeInvitations(int panelId)
    {
        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panelId).ToList();
        var panel = panelManager.GetPanelById(panelId);
        var result = utilityManager.CalculateRecruitment(panel.TotalNeededPanelmembers, criteria);
        
        var invitations = utilityManager.GenerateInvitations(result.TotalNeededInvitations, criteria, panel);
        panel.Invitations = invitations.ToList();
        panelManager.EditPanel(panel);
        return Ok();
    }
}