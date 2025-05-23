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

        List<MeetRecDto> meetRecs = new List<MeetRecDto>();
        
        foreach (var meeting in meetings)
        {
            int totalVotable = 0;
            List<int> recIds = new List<int>();
            List<string> recTitles = new List<string>();
            List<string> recDescriptions = new List<string>();
            List<bool> recAnon = new List<bool>();
            List<bool> recVotable = new List<bool>();
            List<int> recVotes = new List<int>();
            List<int> recVotesFor = new List<int>();
            List<int> recVotesAgainst = new List<int>();
            List<double> recNeededPercentages = new List<double>();
            foreach (var recommendation in meeting.Recommendations)
            {
                recIds.Add(recommendation.Id);
                recTitles.Add(recommendation.Title);
                recDescriptions.Add(recommendation.Description);
                recAnon.Add(recommendation.IsAnonymous);
                recVotable.Add(recommendation.IsVotable);
                recVotes.Add(recommendation.Votes);
                recNeededPercentages.Add(recommendation.NeededPercentage);
                recVotesFor.Add(recommendation.UserVotes.Count(uv => uv.Recommended));
                recVotesAgainst.Add(recommendation.UserVotes.Count(uv => !uv.Recommended));
                if (recommendation.IsVotable) totalVotable++;
            }
            meetRecs.Add(new MeetRecDto
            {
                MeetingTitle = meeting.Title,
                MeetingId = meeting.Id,
                AmountVotable = totalVotable,
                RecIds = recIds,
                RecTitles = recTitles,
                RecDescriptions = recDescriptions,
                RecAnon = recAnon,
                RecVotable = recVotable,
                RecVotes = recVotes,
                RecNeededPercentages = recNeededPercentages,
                RecVotesFor = recVotesFor,
                RecVotesAgainst = recVotesAgainst
            });
        }
        return Ok(meetRecs);
    }
}