using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.DTO;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JsonSerializer=System.Text.Json.JsonSerializer;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

public class PanelController(
    IPanelManager panelManager,
    IDrawManager drawManager,
    IUserProfileManager userProfileManager,
    IUtilityManager utilityManager) : Controller
{
    [HttpGet]
    [Authorize]
    public IActionResult Index(int id)
    {
        Panel panel = panelManager.GetPanelByIdWithRecommendations(id);
        
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
    
    [HttpPost]
    [Authorize(Roles = "Organization, Admin")]
    public IActionResult CreatePanel(CreatePanelViewModel model)
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
                        subCriteria.Add(drawManager.AddSubCriteria(sub.Name, sub.Percentage));
                    }
                }
                criteria.Add(drawManager.AddCriteria(crit.Name, subCriteria));
            }
        }
        
        Panel newPanel = panelManager.AddPanel(model.Name, model.Description, criteria, model.Result.TotalAvailablePotentialPanelmembers);
        var invitations = utilityManager.GenerateInvitations(model.Result.ReservePotPanelmembers, criteria, newPanel);
        newPanel.Invitations = invitations.ToList();
        panelManager.EditPanel(newPanel);
        return RedirectToAction("Index","Panel",new { id = newPanel.Id });
    }
    
    [Authorize]
    public IActionResult UserPanel(string returnUrl)
    {
        var user = userProfileManager.GetUserByPrincipalWithProfileAndPanels(User);

        if (user is null)
        {
            return Unauthorized();
        }

        if (user.UserType == UserType.Admin)
        {
            return LocalRedirect(returnUrl);
        }

        var panels = user.UserType == UserType.Member ? user.MemberProfile.Panels : panelManager.GetAllPanels().ToList();

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

    [Authorize]
    public IActionResult Invitations(int panelId)
    {
        var invitations = drawManager.GetAllInvitationsByPanelId(panelId);
        return View(invitations);
    }
}