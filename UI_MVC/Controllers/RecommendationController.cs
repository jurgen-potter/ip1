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

    public IActionResult AddRecommendation()
    {
        return View();
    }

    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public IActionResult AddRecommendation(string title, string description)
    {
        var panel = panelManager.GetPanelById(1); //gehardcode omdat panels nog niet echt gelinked zijn aan gebruikers.
        if (ModelState.IsValid)
        {
            panelManager.AddRecommendationOfPanel(title, description, panel);
            return RedirectToAction(nameof(Index));
        }
        return View("Index");
    }
}