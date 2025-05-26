using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.Panels;
using Google;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

public class MeetingController(
    IMeetingManager meetingManager,
    IPanelManager panelManager,
    StorageClient storageClient) : Controller

{
    private readonly IMeetingManager _meetingManager = meetingManager;
    private readonly IPanelManager _panelManager = panelManager;
    private readonly StorageClient _storageClient = storageClient;
    private readonly string _bucketName = "whimp24-bucket";
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        var meeting = meetingManager.GetMeetingByIdWithRecommendations(id);
        var panel = panelManager.GetPanelById(meeting.PanelId);
        
        var documents = new List<string>();

        foreach (var docName in meeting.DocumentNames)
        {
            var objectName = $"{id}/{docName}";

            try
            {
                var obj = await _storageClient.GetObjectAsync(_bucketName, objectName);
                var publicUrl = $"https://storage.googleapis.com/{_bucketName}/{objectName}";
                documents.Add(publicUrl);
            }
            catch (Google.GoogleApiException e) when (e.Error.Code == 404)
            {
                // Bestand niet gevonden in bucket
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
            var objectName = $"{meetingId}/{file.FileName}";
            using var stream = file.OpenReadStream();
            try
            {
                await _storageClient.UploadObjectAsync(_bucketName, objectName, file.ContentType, stream);
            }
            catch (GoogleApiException ex)
            {
                // Log dit naar bestand of console
                Console.WriteLine($"Upload failed: {ex.Message}");
                throw; 
            }

            var meeting = _meetingManager.GetMeetingById(meetingId);
            if (!meeting.DocumentNames.Contains(file.FileName))
            {
                meeting.DocumentNames.Add(file.FileName);
                _meetingManager.EditMeeting(meeting);
            }
        }
        return RedirectToAction("Details", new { id = meetingId });
    }

    [HttpPost]
    public IActionResult RemoveDocument(int meetingId, string fileName)
    {
        var objectName = $"{meetingId}/{fileName}";

        try
        {
            _storageClient.DeleteObject(_bucketName, objectName);
        }
        catch (Google.GoogleApiException e) when (e.Error.Code == 404)
        {
            // Bestand bestaat niet, negeer
        }

        var meeting = _meetingManager.GetMeetingById(meetingId);
        if (meeting.DocumentNames.Contains(fileName))
        {
            meeting.DocumentNames.Remove(fileName);
            _meetingManager.EditMeeting(meeting);
        }

        return RedirectToAction("Details", new { id = meetingId });
    }
}