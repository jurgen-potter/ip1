using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class PanelController : Controller
{
    private readonly IPanelManager _panelManager;
    private readonly IDrawManager _drawManager;
    private readonly ApplicationUserManager _userManager;

    public PanelController(IPanelManager panelManager, IDrawManager drawManager, ApplicationUserManager userManager)
    {
        _panelManager = panelManager;
        _drawManager = drawManager;
        _userManager = userManager;
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
            CoverImagePath = panel.CoverImagePath
        };

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
    public async Task<IActionResult> CreatePanel(CreatePanelViewModel model)
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
        
        var organization = await _userManager.GetUserWithProfilesAndPanelsAsync(User);
        Panel newPanel = _panelManager.AddPanel(model.Name, model.Description, criteria, organization.OrganizationProfile);
        organization.OrganizationProfile.Panels.Add(newPanel);
        await _userManager.UpdateAsync(organization);

        if (criteria.Count != 0)
        {
            foreach (Criteria criterion in criteria)
            {
                criterion.Panel = newPanel;
                _panelManager.EditCriteria(criterion);
            }
        }
        
        return RedirectToAction("Index","Panel",new {panelId=newPanel.Id});
    }
    
    [Authorize]
    public async Task<IActionResult> UserPanel()
    {
        var user = await _userManager.GetUserWithProfilesAndPanelsAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        var panels = user.UserType == UserType.Member ? user.MemberProfile.Panels : user.OrganizationProfile.Panels;

        if (panels.Count == 1)
        {
            return RedirectToAction("Index", new { id = panels.First().Id });
        }
        else if (panels.Count != 0)
        {
            throw new NotImplementedException(); // Keuze scherm
        }
        else // Count == 0
        {
            throw new NotImplementedException(); // Home
        }
    }
}