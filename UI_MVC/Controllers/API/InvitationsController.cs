using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.Draws;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/{tenantId}/api/[controller]")]
[Authorize(Roles = "Organization")]
public class InvitationsController(IPanelManager panelManager, IDrawManager drawManager, IUtilityManager utilityManager) : ControllerBase
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
        var panel = panelManager.GetPanelByIdWithInvitations(model.PanelId);
        var buckets = model.Buckets.Where(b => !b.IsSufficient);

        var missingMembers = 0;
        foreach (var bucket in buckets)
        {
            missingMembers += bucket.Count - bucket.RegisteredCount;
        }
        var result = utilityManager.CalculateRecruitment(missingMembers, criteria);

        var invitations = utilityManager.GenerateInvitations(result.TotalNeededInvitations, criteria, panel, panel.Invitations.Max(i => i.Batch) + 1);
        panel.Invitations.AddRange(invitations.ToList());
        panelManager.EditPanel(panel);
        return Ok();
    }

    [HttpGet("download/{panelId}/{batchNr}")]
    public IActionResult DownloadInvitationsExcel(int panelId, int batchNr)
    {
        var invitations = drawManager.GetAllInvitationsByPanelId(panelId);

        byte[] excelFile = utilityManager.GenerateExcelWithQrCodes(invitations.Where(i => i.Batch == batchNr));
        
        string fileName = $"Uitnodigingen_{DateTime.Now:yyyy-MM-dd}.xlsx";

        return File(excelFile, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}