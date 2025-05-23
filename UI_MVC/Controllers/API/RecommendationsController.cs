using System.Security.Claims;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;


[ApiController]
[Route("{tenant}/api/[controller]")]
[Authorize]
public class RecommendationsController(
    IPanelManager panelManager,
    IUserProfileManager userProfileManager) : ControllerBase
{
    [HttpGet("userVotes")]
    public IActionResult GetUserVotes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }
        var votedRecommendation = panelManager.GetUserVotesById(userId);
        return Ok(votedRecommendation);
    }

    [HttpPost("vote")]
    public IActionResult Vote([FromBody] VoteDto voteDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = userProfileManager.GetUserById(userId);
        if (string.IsNullOrEmpty(userId) || member.UserType != UserType.Member)
        {
            return Unauthorized();
        }

        var recommendation = panelManager.GetRecommendationByIdWithVotes(voteDto.Id);
        if (recommendation == null)
        {
            return NotFound();
        }

        var hasVoted = panelManager.DoesUserVoteExist(member, recommendation);
        if (hasVoted)
        {
            return BadRequest(new { message = "U heeft al gestemd op deze aanbeveling" });
        }

        panelManager.AddUserVote(member, recommendation, voteDto.Recommended);

        return Ok(new { id = recommendation.Id, votes = recommendation.Votes });
    }

    [HttpPost("remove-vote")]
    public IActionResult RemoveVote([FromBody] VoteDto voteDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var member = userProfileManager.GetUserById(userId);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var recommendation = panelManager.GetRecommendationByIdWithVotes(voteDto.Id);

        if (recommendation == null)
        {
            return NotFound();
        }

        var hasVoted = panelManager.DoesUserVoteExist(member, recommendation);

        if (!hasVoted)
        {
            return BadRequest(new { message = "U heeft niet gestemd op deze aanbeveling" });
        }

        panelManager.RemoveUserVote(member, recommendation);
        panelManager.EditRecommendation(recommendation);

        return Ok(new { id = recommendation.Id, votes = recommendation.Votes });
    }

    [HttpGet("votableRecs/{panelId}")]
    public IActionResult GetVotableRecommendations(int panelId)
    {
        var recs = panelManager.GetVotableRecommendationsByIdWithVotes(panelId);
        return Ok(recs);
    }

    [HttpGet("unvotableRecs/{panelId}")]
    public IActionResult GetUnvotableRecommendations(int panelId)
    {
        var recs = panelManager.GetUnvotableRecommendationsByIdWithVotes(panelId);
        return Ok(recs);
    }
}