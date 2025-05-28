using System.Security.Claims;
using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Tenancy;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.DTO;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

public class PanelController(
    IPanelManager panelManager,
    IDrawManager drawManager,
    IUserProfileManager userProfileManager,
    ITenantManager tenantManager,
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
            PanelPartcipants = panel.MemberCount,
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
                        Description = rec.Description,
                        IsDone = rec.IsDone
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
        if (!ModelState.IsValid)
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

        Panel newPanel = panelManager.AddPanel(model.Name, model.Description, criteria,
            model.Result.TotalNeededPanelmembers);
        var invitations = utilityManager.GenerateInvitations(model.Result.TotalNeededInvitations, criteria, newPanel);
        newPanel.Invitations = invitations.ToList();
        panelManager.EditPanel(newPanel);
        return RedirectToAction("Index", "Panel", new { id = newPanel.Id });
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

        var panels = user.UserType == UserType.Member
            ? user.MemberProfile.Panels
            : panelManager.GetAllPanels().ToList();

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

    [Authorize(Roles = "Organization")]
    public IActionResult Members(int panelId)
    {
        var panel = panelManager.GetPanelByIdWithMembers(panelId);
        var users = panel.Members;
        return View(panel);
    }

    [HttpGet]
    public IActionResult Details(int panelId)
    {
        Panel panel = panelManager.GetPanelByIdWithRecommendationsAndPosts(panelId);

        var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "panelUploads", panelId.ToString());
        var documents = new List<string>();

        if (Directory.Exists(uploadsPath))
        {
            foreach (var docUrl in panel.PublicDocumentNames)
            {
                var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var physicalPath = Path.Combine(webRoot, docUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(physicalPath))
                {
                    documents.Add(docUrl);
                }
            }
        }
        
        PanelViewModel model = new PanelViewModel()
        {
            PanelId = panel.Id,
            Name = panel.Name,
            Description = panel.Description,
            StartDate = panel.StartDate,
            EndDate = panel.EndDate,
            CoverImagePath = panel.CoverImagePath,
            PublicDocumentNames = documents,
            ShowRejected = panel.ShowRejectedRecommendations
        };
        
        foreach (Meeting meeting in panel.Meetings.OrderBy(m => m.Date))
        {
            MeetingViewModel meetingViewModel = new MeetingViewModel
            {
                Id = meeting.Id,
                Title = meeting.Title,
                Date = meeting.Date,
            };

            if (meeting.Recommendations != null)
            {
                foreach (Recommendation rec in meeting.Recommendations.Where(rec => rec.Accepted))
                {
                    meetingViewModel.Recommendations.Add(new RecommendationViewModel
                    {
                        Id = rec.Id,
                        Title = rec.Title,
                        Description = rec.Description,
                        IsDone = rec.IsDone
                    });
                }

                if (panel.ShowRejectedRecommendations)
                {
                    foreach (Recommendation rec in meeting.Recommendations.Where(rec => !rec.Accepted))
                    {
                        meetingViewModel.RejectedRecommendations.Add(new RecommendationViewModel
                        {
                            Id = rec.Id,
                            Title = rec.Title,
                            Description = rec.Description,
                            IsDone = rec.IsDone
                        });
                    }
                }
            }

            model.Meetings.Add(meetingViewModel);
        }

        foreach (Post post in panel.Posts)
        {
            PostViewModel postViewModel = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                DatePosted = post.DatePosted
            };
            model.Posts.Add(postViewModel);
        }

        if (User.Identity.IsAuthenticated)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = userProfileManager.GetUserByIdWithProfile(userId);
            string currentUserTenantId = null;

            if (user.UserType == UserType.Organization && user.OrganizationProfile != null)
            {
                currentUserTenantId = user.OrganizationProfile.TenantId;
            }

            // else if (user.UserType == UserType.Member && user.MemberProfile != null) //mss als leden dat ook moeten kunnen anapassen
            // {
            //     currentUserTenantId = user.MemberProfile.TenantId;
            // }
            model.CanManagePanel = User.IsInRole("Organization") && panel.TenantId == currentUserTenantId;
        }


        return View(model);
    }

    public IActionResult ViewAll(string searchPanelName, string searchOrganisationName)
    {
        var allPanels = panelManager.GetAllPanelsWithoutTentant(); 

        IEnumerable<Panel>
            filteredPanels = allPanels.AsEnumerable(); 

        if (!string.IsNullOrEmpty(searchPanelName))
        {
            filteredPanels = filteredPanels.Where(p =>
                p.Name != null &&
                p.Name.Contains(searchPanelName, StringComparison.OrdinalIgnoreCase));
        }
        
        var allTenants = tenantManager.GetAllTenants(); 
        var tenantNameLookup = allTenants.ToDictionary(t => t.Id, t => t.Name ?? "Onbekende Organisatie");

        if (!string.IsNullOrEmpty(searchOrganisationName))
        {
            // Vind TenantIds die overeenkomen met de gezochte organisatienaam
            var matchingTenantIds = allTenants
                .Where(t => t.Name != null &&
                            t.Name.Contains(searchOrganisationName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Id)
                .ToList();

            filteredPanels = filteredPanels.Where(p =>
                !string.IsNullOrEmpty(p.TenantId) && matchingTenantIds.Contains(p.TenantId));
        }

        var panelSummaries = filteredPanels.Select(p =>
        {
            string status;
            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (p.EndDate < today) // Panel.EndDate is DateOnly (niet nullable)
                status = "Afgelopen";
            else if (p.StartDate > today)
                status = "Gepland";
            else
                status = "Actief";

            tenantNameLookup.TryGetValue(p.TenantId ?? string.Empty, out string orgName);

            return new PanelSummaryViewModel
            {
                TenantId = p.TenantId,
                PanelId = p.Id,
                PanelName = p.Name ?? "N/A",
                OrganisationName =
                    orgName ?? (string.IsNullOrEmpty(p.TenantId) ? "Geen Tenant" : $"Tenant ID: {p.TenantId}"),
                StartDate = p.StartDate, 
                EndDate = p.EndDate, 
                Status = status
            };
        }).OrderByDescending(p => p.StartDate).ToList();

        var viewModel = new AllPanelsViewModel
        {
            Panels = panelSummaries,
            SearchPanelName = searchPanelName,
            SearchOrganisationName = searchOrganisationName
        };

        return View(viewModel); // Zorg dat je een View hebt genaamd "ViewAll.cshtml" of pas de naam aan.
    }

    [HttpGet]
    public IActionResult Edit(int panelId)
    {
        return View(panelId);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file, int panelId)
    {
        if (file != null && file.Length > 0)
        {
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "panelUploads", panelId.ToString());
            Directory.CreateDirectory(uploads);

            var filePath = Path.Combine(uploads, Path.GetFileName(file.FileName));
            bool exists = System.IO.File.Exists(filePath);
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (!exists)
            {
                var panel = panelManager.GetPanelById(panelId);
                panel.PublicDocumentNames.Add($"/panelUploads/{panelId.ToString()}/{file.FileName}");
                panelManager.EditPanel(panel);
            }
        }

        return RedirectToAction("Details", new { panelId = panelId });
    }
    
    [HttpPost]
    public IActionResult RemoveDocument(int panelId, string fileUrl)
    {
        var webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var physicalPath = Path.Combine(webRoot, fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (System.IO.File.Exists(physicalPath))
        {
            System.IO.File.Delete(physicalPath);
        }
        
        var panel = panelManager.GetPanelById(panelId);
        panel.PublicDocumentNames.Remove(fileUrl);
        panelManager.EditPanel(panel);

        return RedirectToAction("Details", new { panelId = panelId });
    }
}