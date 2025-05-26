using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("{tenant}/api/[controller]")]
[Authorize]
public class MeetingsController(IPanelManager panelManager) : ControllerBase
{
    [HttpGet("getMeetings/{panelId}")]
    public IActionResult GetMeetings(int panelId)
    {
        var meetings = panelManager.GetMeetingsById(panelId);

        List<MeetingDto> meetRecs = new List<MeetingDto>();
        
        foreach (var meeting in meetings)
        {
            int totalVotable = 0;
            List<RecDto> recs = new List<RecDto>();
            foreach (var recommendation in meeting.Recommendations)
            {
                RecDto rec = new RecDto()
                {
                    Id = recommendation.Id,
                    Title = recommendation.Title,
                    Description = recommendation.Description,
                    Anon = recommendation.IsAnonymous,
                    Votable = recommendation.IsVotable,
                    Votes = recommendation.Votes,
                    NeededPercentages = recommendation.NeededPercentage,
                    VotesFor = recommendation.UserVotes.Count(uv => uv.Recommended),
                    VotesAgainst = recommendation.UserVotes.Count(uv => !uv.Recommended)
                };
                if (recommendation.IsVotable) totalVotable++;
                recs.Add(rec);
            }
            meetRecs.Add(new MeetingDto
            {
                MeetingTitle = meeting.Title,
                MeetingId = meeting.Id,
                Participants = meeting.PanelParticipants,
                AmountVotable = totalVotable,
                Recs = recs
            });
        }
        return Ok(meetRecs);
    }
}