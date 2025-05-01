using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class MeetingController : Controller
{
    private readonly IPanelManager _panelManager;

    public MeetingController(IPanelManager panelManager)
    {
        _panelManager = panelManager;
    }
    
    // GET: Meeting/Detail/{dateString}
    public IActionResult Detail(string id, int panelId = 1)
    {
        if (string.IsNullOrEmpty(id) || id.Length != 8)
        {
            return RedirectToAction("Index", "Panel", new { panelId });
        }

        // Parse the date from the id (yyyyMMdd format)
        if (!int.TryParse(id.Substring(0, 4), out int year) ||
            !int.TryParse(id.Substring(4, 2), out int month) ||
            !int.TryParse(id.Substring(6, 2), out int day))
        {
            return RedirectToAction("Index", "Panel", new { panelId });
        }

        DateOnly meetingDate;
        try
        {
            meetingDate = new DateOnly(year, month, day);
        }
        catch (Exception)
        {
            return RedirectToAction("Index", "Panel", new { panelId });
        }

        // Get panel with meetings
        Panel panel = _panelManager.GetPanelByIdWithRecommendations(panelId);
        
        // Find the specific meeting
        Meeting? meeting = panel.Meetings.FirstOrDefault(m => m.Date == meetingDate);
        
        if (meeting == null)
        {
            return RedirectToAction("Index", "Panel", new { panelId });
        }

        // Create view model
        MeetingDetailViewModel model = new MeetingDetailViewModel
        {
            PanelId = panel.Id,
            PanelName = panel.Name,
            MeetingDate = meeting.Date,
            Recommendations = meeting.Recommendations.Select(r => new RecommendationViewModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description
            }).ToList()
        };

        return View(model);
    }
}