using System.Security.Claims;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;
[Authorize]
public class RecommendationController(IPanelManager panelManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId)
    {
        var panel = panelManager.GetPanelByIdWithRecommendations(panelId);
        var meetings = panel.Meetings;
        return View(meetings);
    }

    [HttpGet]
    [Authorize(Roles = "Organization")] 
    public IActionResult GetVoters(int recommendationId)
    {
        var recommendation = panelManager.GetRecommendationWithVoters(recommendationId);

        var votes = recommendation?.UserVotes ?? Enumerable.Empty<UserVote>();
        return PartialView("_VotersList", votes);
    }
    
    [HttpPost]
    [Authorize(Roles = "Organization")]
    public IActionResult StopVoting(int id)
    {
        var recommendation = panelManager.GetRecommendationById(id);
        if (recommendation == null)
            return NotFound();

        recommendation.IsVotable = false;
        panelManager.ChangeRecommendation(recommendation);
        
        return Ok();
    }
}