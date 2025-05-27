using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
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
}