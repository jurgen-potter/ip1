using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingManager _meetingManager;
        private readonly IPanelManager _panelManager;

        public MeetingController(IMeetingManager meetingManager, IPanelManager panelManager)
        {
            _meetingManager = meetingManager;
            _panelManager = panelManager;
        }
        
        [HttpGet]
        public IActionResult Details(int id, int panelId)
        {
            var meeting = _meetingManager.GetMeetingByIdWithRecommendations(id);

            var panel = _panelManager.GetPanelById(panelId);

            var model = new MeetingDetailViewModel
            {
                PanelId = panelId,
                PanelName = panel.Name,
                MeetingDate = meeting.Date,
                Recommendations = new List<RecommendationViewModel>()
            };

            if (meeting.Recommendations != null)
            {
                foreach (var rec in meeting.Recommendations)
                {
                    model.Recommendations.Add(new RecommendationViewModel
                    {
                        Id = rec.Id,
                        Title = rec.Title,
                        Description = rec.Description
                    });
                }
            }

            return View(model);
        }
    }
}