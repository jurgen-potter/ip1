using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using JsonSerializer=System.Text.Json.JsonSerializer;

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
    public IActionResult Index(int id)
    {
        Panel panel = _panelManager.GetPanelByIdWithRecommendations(id);
        
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
                Id = meeting.Id,
                Title = meeting.Title,
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

        foreach (Meeting meeting in panel.Meetings.OrderBy(m => m.Date))
        {
            foreach (Recommendation recommendation in meeting.Recommendations)
            {
                RecommendationViewModel recommendationModel = new RecommendationViewModel
                {
                    Id = recommendation.Id,
                    Title = recommendation.Title,
                    Description = recommendation.Description
                };
                model.Recommendations.Add(recommendationModel);
            }
        }

        return View(model);
    }

    //[Authorize(Roles = "Organization, Admin")]
    [HttpPost]
    public IActionResult CreatePanelFromResult(ResultViewModel resultViewModel)
    {
        if (!User.Identity.IsAuthenticated)
        {
            TempData["ResultViewModel"] = JsonConvert.SerializeObject(resultViewModel);
            return RedirectToPage("/Account/Login", new { area = "Identity", returnUrl = Url.Action("RestorePanelData", "Panel") });
        }
        
        if (!User.IsInRole("Organization") && !User.IsInRole("Admin"))
        {
            return Forbid();
        }
        
        CreatePanelViewModel model = new CreatePanelViewModel()
        {
            Result = resultViewModel
        };
        return View("CreatePanel", model);
    }
    
    [Authorize(Roles = "Organization, Admin")]
    public IActionResult RestorePanelData()
    {
        if (!TempData.ContainsKey("ResultViewModel"))
        {
            return RedirectToAction("Index", "Recruitment");
        }

        var resultModelJson = TempData["ResultViewModel"] as string;
        var resultViewModel = JsonConvert.DeserializeObject<ResultViewModel>(resultModelJson);

        var model = new CreatePanelViewModel()
        {
            Result = resultViewModel
        };

        return View("CreatePanel", model);
    }
    
    [Authorize(Roles = "Organization, Admin")]
    [HttpPost]
    public async Task<IActionResult> CreatePanel(CreatePanelViewModel model)
    {
        if(!ModelState.IsValid)
            return View(model);

        List<Criteria> criteria = new List<Criteria>();

        if (model.Result.Criteria != null)
        {
            foreach (var crit in model.Result.Criteria)
            {
                List<SubCriteria> subCriteria = new List<SubCriteria>();
                foreach (var sub in crit.SubCriteria)
                {
                    if (sub.Percentage > 0)
                    {
                        subCriteria.Add(_drawManager.AddSubCriteria(sub.Name, sub.Percentage));
                    }
                }
                criteria.Add(_drawManager.AddCriteria(crit.Name, subCriteria));
            }
        }
        
        var organization = await _userManager.GetUserWithProfilesAsync(User);
        Panel newPanel = _panelManager.AddPanel(model.Name, model.Description, criteria);
        await _userManager.UpdateAsync(organization);
        var invitations = _drawManager.AddInvitations(model.Result.ReservePotPanelmembers, criteria, newPanel);
        newPanel.Invitations = invitations.ToList();
        _panelManager.ChangePanel(newPanel);
        return RedirectToAction("Index","Panel",new { id = newPanel.Id });
    }
    
    [Authorize]
    public async Task<IActionResult> UserPanel(string returnUrl)
    {
        var user = await _userManager.GetUserWithProfilesAndPanelsAsync(User);

        if (user is null)
        {
            return Unauthorized();
        }

        if (user.UserType == UserType.Admin)
        {
            return LocalRedirect(returnUrl);
        }

        var panels = user.UserType == UserType.Member ? user.MemberProfile.Panels : _panelManager.GetAllPanels().ToList();

        if (panels.Count == 1)
        {
            return RedirectToAction("Index", new { id = panels.First().Id });
        }
        else if (panels.Count != 0)
        {
            var panelsData = new List<PanelDto>();
            foreach (var panel in panels)
            {
                panelsData.Add(new PanelDto()
                {
                    Id = panel.Id,
                    Name = panel.Name
                });
            }
            TempData["Panels"] = JsonSerializer.Serialize(panelsData);
            
            return RedirectToAction("PanelSelect");
        }
        else // Count == 0
        {
            return RedirectToAction("Index", "Home");
        }
    }

    [Authorize]
    public IActionResult PanelSelect()
    {
        if (TempData["Panels"] is string serializedPanels)
        {
            var panels = JsonSerializer.Deserialize<List<PanelDto>>(serializedPanels);

            if (panels != null)
            {
                var viewModel = new PanelSelectViewModel
                {
                    Panels = panels.Select(p => new PanelSelectOptionViewModel
                    {
                        Id = p.Id,
                        Name = p.Name
                    }).ToList()
                };

                return View(viewModel);
            }
        }
        
        return RedirectToAction("UserPanel", new { returnUrl = "/" });
    }
}