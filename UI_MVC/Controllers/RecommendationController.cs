using System.Security.Claims;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;
[Authorize]
public class RecommendationController(IPanelManager panelManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        var panel = panelManager.GetPanelByIdWithRecommendations(panelId);
        var recommendations = panel.Recommendations;
        
        return View(recommendations);
    }
    
    [HttpGet]
    public IActionResult GetUserVotes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        // Haal alle aanbevelingsIds op waarop de gebruiker heeft gestemd
        var votedRecommendationIds = panelManager.GetVotedRecommendationIdsByUser(userId);
        
        return Json(votedRecommendationIds);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Vote([FromBody] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var recommendation = panelManager.GetRecommendationById(id);
        if (recommendation == null)
        {
            return NotFound();
        }
        
        var hasVoted = panelManager.HasUserVotedForRecommendation(userId, id);
        if (hasVoted)
        {
            return BadRequest(new { message = "U heeft al gestemd op deze aanbeveling" });
        }
        
        panelManager.AddVoteToRecommendation(userId, id);
        
        recommendation = panelManager.GetRecommendationById(id);
        
        return Json(new { id = recommendation.Id, votes = recommendation.Votes });
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult RemoveVote([FromBody] int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        
        var recommendation = panelManager.GetRecommendationById(id);
        if (recommendation == null)
        {
            return NotFound();
        }

        // Controleer of gebruiker heeft gestemd
        var hasVoted = panelManager.HasUserVotedForRecommendation(userId, id);
        if (!hasVoted)
        {
            return BadRequest(new { message = "U heeft niet gestemd op deze aanbeveling" });
        }

        panelManager.RemoveVoteFromRecommendation(userId, id);

        recommendation = panelManager.GetRecommendationById(id);

        return Json(new { id = recommendation.Id, votes = recommendation.Votes });
    }

    public IActionResult Create()
    {
        return View();
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AddRecommendation(Recommendation recommendation, Panel panel)
    {
        if (ModelState.IsValid)
        {
            panelManager.AddRecommendationOfPanel(recommendation, panel);
            return RedirectToAction(nameof(Index));
        }
        return View(recommendation);
    }
}