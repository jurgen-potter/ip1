using System.Security.Claims;
using CitizenPanel.BL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;


[ApiController]
[Route("/api/[controller]")]
[Authorize]
public class RecommendationsController(IPanelManager panelManager) : ControllerBase
{
    
    [HttpGet("votes")]
    public IActionResult GetUserVotes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var votedRecommendationIds = panelManager.GetVotedRecommendationIdsByUser(userId);
        return Ok(votedRecommendationIds);
    }

    [HttpPost("vote")]
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

        return Ok(new { id = recommendation.Id, votes = recommendation.Votes });
    }

    [HttpPost("remove-vote")]
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

        var hasVoted = panelManager.HasUserVotedForRecommendation(userId, id);
        if (!hasVoted)
        {
            return BadRequest(new { message = "U heeft niet gestemd op deze aanbeveling" });
        }

        panelManager.RemoveVoteFromRecommendation(userId, id);
        recommendation = panelManager.GetRecommendationById(id);

        return Ok(new { id = recommendation.Id, votes = recommendation.Votes });
    }
}