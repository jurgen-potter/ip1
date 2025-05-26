using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models.DTO;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;
[ApiController]
[Route("{tenant}/api/[controller]")]
public class PanelsController(IPanelManager panelManager) : ControllerBase
{
    [HttpPost("edit")]
    public IActionResult EditPanel([FromBody] PanelDto model)
    {
        var panel = panelManager.GetPanelByIdWithRecommendationsWithoutTenant(model.Id);
        panel.ShowRejectedRecommendations = model.ShowRejected;
        panelManager.EditPanel(panel);
        
        List<RecDto> recs = new List<RecDto>();
        foreach (var meeting in panel.Meetings)
        {
            foreach (var recommendation in meeting.Recommendations.Where(r => !r.IsVotable && !r.Accepted))
            {
                RecDto recommendationDto = new RecDto()
                {
                    Id = recommendation.Id,
                    Title = recommendation.Title,
                    Description = recommendation.Description
                };
                recs.Add(recommendationDto);
            }
        }
        
        return Ok(recs);
    }
}