using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web;
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

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public IActionResult Create(CreateMeetingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState });
            }
            
            if (model.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest(new { success = false, errors = "Meeting date cannot be in the past" });
            }
            
            var meeting = _meetingManager.AddMeeting(model.Title, model.Date,model.PanelId);

            return Json(new
            {
                success = true,
                meeting = new
                {
                    id = meeting.Id,
                    title = meeting.Title,
                    date = meeting.Date.ToString("yyyy-MM-dd")
                }
            });
            }
        }
    }