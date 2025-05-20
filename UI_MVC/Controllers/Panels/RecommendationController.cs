using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

[Authorize]
public class RecommendationController(
    IPanelManager panelManager) : Controller
{
    public IActionResult Index(int panelId)
    {
        var panel = panelManager.GetPanelByIdWithRecommendationsAndVotes(panelId);
        var meetings = panel.Meetings;
        return View(meetings);
    }

    [HttpGet]
    [Route("{tenant}/Recommendation/GetVoters/{recommendationId}")]
    [Authorize(Roles = "Organization")] 
    public IActionResult GetVoters(int recommendationId)
    {
        var recommendation = panelManager.GetRecommendationByIdWithVoters(recommendationId);

        var votes = recommendation?.UserVotes ?? Enumerable.Empty<UserVote>();
        return PartialView("_VotersList", votes);
    }
    
    [HttpGet("{tenant}/Recommendation/StopVoting/{id}")]
    [Authorize(Roles = "Organization")]
    public IActionResult StopVoting(int id)
    {
        var recommendation = panelManager.GetRecommendationByIdWithVotes(id);
        if (recommendation == null)
            return NotFound();

        recommendation.IsVotable = false;
        panelManager.EditRecommendation(recommendation);
        
        return Ok();
    }
}