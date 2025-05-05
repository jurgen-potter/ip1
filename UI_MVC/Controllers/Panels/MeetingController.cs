using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Panels
{
    public class MeetingController(
        IMeetingManager meetingManager,
        IPanelManager panelManager) : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Details(int id, int panelId)
        {
            var meeting = meetingManager.GetMeetingByIdWithRecommendations(id);
            
            var panel = panelManager.GetPanelById(panelId);

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
        
        
        [HttpGet]
        [Authorize(Roles = "Organization")]
        public IActionResult AddRecommendation(int meetingId, int panelId)
        {
            var model = new AddRecommendationViewModel
            {
                MeetingId = meetingId,
                PanelId = panelId
            };
    
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Organization")]
        public IActionResult AddRecommendation(AddRecommendationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var meeting = meetingManager.GetMeetingByIdWithRecommendations(model.MeetingId);
    
            var recommendation = new Recommendation()
            {
                Title = model.Title,
                Description = model.Description,
                NeededVotes = model.NeededVotes,
                IsAnonymous = model.IsAnonymous,
                TenantId = meeting.TenantId
            };
    
            meeting.Recommendations.Add(recommendation);
            meetingManager.EditMeeting(meeting);

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
            
            var meeting = meetingManager.AddMeeting(viewModel.Title, viewModel.Date,viewModel.PanelId);

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