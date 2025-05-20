using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

public class MeetingController(
    IMeetingManager meetingManager,
    IPanelManager panelManager) : Controller
{
    [HttpGet]
    [Authorize]
    public IActionResult Details(int id)
    {
        var meeting = meetingManager.GetMeetingByIdWithRecommendations(id);
        var panel = panelManager.GetPanelById(meeting.PanelId);
        
        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "meetingUploads", id.ToString());
        var documents = new List<string>();

        if (Directory.Exists(uploadsPath))
        {
            foreach (var docName in meeting.DocumentNames)
            {
                var fullPath = Path.Combine(uploadsPath, docName);
                if (System.IO.File.Exists(fullPath))
                {
                    documents.Add(docName);
                }
            }
        }

        var model = new MeetingDetailViewModel
        {
            MeetingId = meeting.Id,
            PanelId = meeting.PanelId,
            PanelName = panel.Name,
            MeetingDate = meeting.Date,
            Recommendations = new List<RecommendationViewModel>(),
            DocumentNames = documents
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
        if (!ModelState.IsValid) { return View(model); }
        var meeting = meetingManager.GetMeetingByIdWithRecommendations(model.MeetingId);

        var recommendation = new Recommendation()
        {
            Title = model.Title,
            Description = model.Description,
            NeededVotes = model.NeededVotes,
            IsAnonymous = model.IsAnonymous
        };

        meeting.Recommendations.Add(recommendation);
        meetingManager.EditMeeting(meeting);

        return RedirectToAction("Details", new { id = model.MeetingId, panelId = model.PanelId });
    }


    [HttpPost]
    [Authorize(Roles = "Organization")]
    public IActionResult Create(CreateMeetingViewModel viewModel)
    {
        if (!ModelState.IsValid) { return BadRequest(new { success = false, errors = ModelState }); }

        if (viewModel.Date < DateOnly.FromDateTime(DateTime.Now)) { return BadRequest(new { success = false, errors = "Meeting date cannot be in the past" }); }

        var meeting = meetingManager.AddMeeting(viewModel.Title, viewModel.Date, viewModel.PanelId);

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
    
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, int meetingId)
    {
        if (file != null && file.Length > 0)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "meetingUploads", meetingId.ToString());
            Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, Path.GetFileName(file.FileName));
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var meeting = meetingManager.GetMeetingById(meetingId);
            meeting.DocumentNames.Add(file.FileName);
            meetingManager.EditMeeting(meeting);
        }

        return RedirectToAction("Details", new { id = meetingId });
    }
    
    [HttpPost]
    public IActionResult RemoveDocument(int meetingId, string fileName)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "meetingUploads", meetingId.ToString());
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
        
        var meeting = meetingManager.GetMeetingById(meetingId);
        meeting.DocumentNames.Remove(fileName);
        meetingManager.EditMeeting(meeting);

        return RedirectToAction("Details", new { id = meetingId });
    }
}