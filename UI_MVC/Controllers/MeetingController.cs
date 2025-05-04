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
                MeetingId = meeting.Id,
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
        
        
        [Authorize(Roles = "Organization")]
        [HttpGet]
        public IActionResult AddRecommendation(int meetingId, int panelId)
        {
            var model = new AddRecommendationViewModel
            {
                MeetingId = meetingId,
                PanelId = panelId
            };
    
            return View(model);
        }

        [Authorize(Roles = "Organization")]
        [HttpPost]
        public IActionResult AddRecommendation(AddRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var meeting = _meetingManager.GetMeetingByIdWithRecommendations(model.MeetingId);
    
            var recommendation = new Recommendation()
            {
                Title = model.Title,
                Description = model.Description,
                NeededVotes = model.NeededVotes,
                TenantId = meeting.TenantId
            };
    
            meeting.Recommendations.Add(recommendation);
            _meetingManager.EditMeeting(meeting);

            return RedirectToAction("Details", new { id = model.MeetingId, panelId = model.PanelId });
        }
        

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public IActionResult Create(CreateMeetingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, errors = ModelState });
            }
            
            if (viewModel.Date < DateOnly.FromDateTime(DateTime.Now))
            {
                return BadRequest(new { success = false, errors = "Meeting date cannot be in the past" });
            }
            
            var meeting = _meetingManager.AddMeeting(viewModel.Title, viewModel.Date,viewModel.PanelId);

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