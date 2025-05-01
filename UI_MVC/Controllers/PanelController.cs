using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class PanelController : Controller
{
    private readonly IPanelManager _panelManager;
    private readonly IDrawManager _drawManager;

    public PanelController(IPanelManager panelManager, IDrawManager drawManager)
    {
        _panelManager = panelManager;
        _drawManager = drawManager;
    }
    
    // GET
    public IActionResult Index(int panelId = 1)
    {
        Panel panel = _panelManager.GetPanelByIdWithRecommendations(panelId);
        
        PanelViewModel model = new PanelViewModel()
        {
            PanelId = panel.Id,
            Name = panel.Name,
            Description = panel.Description,
            StartDate = panel.StartDate,
            EndDate = panel.EndDate,
            CoverImagePath = panel.CoverImagePath,
        };

        foreach (Meeting meeting in panel.Meetings)
        {
            MeetingViewModel meetingViewModel = new MeetingViewModel
            {
                Date = meeting.Date,
            };
            
            if (meeting.Recommendations != null)
            {
                foreach (Recommendation rec in meeting.Recommendations)
                {
                    meetingViewModel.Recommendations.Add(new RecommendationViewModel
                    {
                        Id = rec.Id,
                        Title = rec.Title,
                        Description = rec.Description
                    });
                }
            }
            model.Meetings.Add(meetingViewModel);
        }

        foreach (Recommendation recommendation in panel.Recommendations)
        {
            RecommendationViewModel recommendationModel = new RecommendationViewModel
            {
                Id = recommendation.Id,
                Title = recommendation.Title,
                Description = recommendation.Description
            };
            model.Recommendations.Add(recommendationModel);
        }

        return View(model);
    }

    [Authorize(Roles = "Organization, Admin")]
    [HttpGet]
    public IActionResult CreatePanel(ResultsViewModel resultsViewModel)
    {
        CreatePanelViewModel model = new CreatePanelViewModel()
        {
            Results = resultsViewModel
        };
        return View(model);
    }
    
    [Authorize(Roles = "Organization, Admin")]
    [HttpPost]
    public IActionResult CreatePanel(CreatePanelViewModel model)
    {
        if(!ModelState.IsValid)
            return View(model);

        List<Criteria> criteria = new List<Criteria>();

        if (model.Results.CriteriaResults != null)
        {
            foreach (CriteriaResult criteriaResult in model.Results.CriteriaResults)
            {
                List<SubCriteria> subCriteria = new List<SubCriteria>();
                foreach (SubCriteriaResult subResult in criteriaResult.SubResults)
                {
                    subCriteria.Add(_drawManager.AddSubCriteria(subResult.Name, subResult.Percentage));
                }
                criteria.Add(_drawManager.AddCriteria(criteriaResult.Name, subCriteria));
            }
        }
        
        Panel newPanel = _panelManager.AddPanel(model.Name, model.Description, criteria);

        if (criteria.Count != 0)
        {
            foreach (Criteria criterion in criteria)
            {
                criterion.Panel = newPanel;
            }
        }
        
        return RedirectToAction("Index","Panel",new {panelId=newPanel.Id});
    }
}