using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Registrations;
using CitizenPanel.BL.Users;
using CitizenPanel.BL.Utilities;
using CitizenPanel.UI.MVC.Models.Draws;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

public class RegistrationController(
    IRegistrationManager registrationManager,
    IEmailSender mailSender,
    IPanelManager panelManager,
    IDrawManager drawManager,
    IUtilityManager utilityManager,
    IUserProfileManager userProfileManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId)
    {
        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panelId);
        var users = drawManager.GetRegisteredInvitationsByPanelId(panelId).ToList();
        var panel = panelManager.GetPanelById(panelId);
        var result = utilityManager.CalculateRecruitment(panel.TotalNeededPanelmembers, criteria);

        var bucketsWithActuals = registrationManager.AssignActualRegistrationsToBuckets(result.Buckets, users);

        var vm = new ResultViewModel
        {
            TotalNeededInvitations = result.TotalNeededInvitations,
            TotalNeededPanelmembers = result.TotalNeededPanelmembers,
            Criteria = criteria.Select(c => new CriteriaViewModel
            {
                Name = c.Name,
                SubCriteria = c.SubCriteria.Select(sc => new SubCriteriaViewModel
                {
                    Name = sc.Name,
                    Percentage = sc.Percentage
                }).ToList()
            }).ToList(),
            Buckets = bucketsWithActuals.Select(b => new BucketViewModel
            {
                CriteriaNames = b.CriteriaNames,
                SubCriteriaNames = b.SubCriteriaNames,
                Count = b.Count,
                RegisteredCount = b.ActualCount
            }).ToList()
        };

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;
        ViewBag.PanelName = panel.Name;

        return View(vm);
    }

    [HttpPost]
    public IActionResult EditMail(int panelId)
    {
        var model = new FinalDrawViewModel
        {
            PanelId = panelId
        };

        return View(model);
    }

    [HttpPost]
    public IActionResult StartFinalDrawPhase(FinalDrawViewModel finalDraw)
    {
        if (!ModelState.IsValid)
        {
            return View("EditMail", finalDraw);
        }

        var panel = panelManager.GetPanelByIdWithInvitations(finalDraw.PanelId);
        registrationManager.StartFinalDraw(panel);

        TempData["SelectedSubject"] = finalDraw.SelectedSubject;
        TempData["SelectedMessage"] = finalDraw.SelectedMessage;
        TempData["ReserveSubject"] = finalDraw.ReserveSubject;
        TempData["ReserveMessage"] = finalDraw.ReserveMessage;
        TempData["NotSelectedSubject"] = finalDraw.NotSelectedSubject;
        TempData["NotSelectedMessage"] = finalDraw.NotSelectedMessage;

        return RedirectToAction(nameof(DrawResults), new { panelId = finalDraw.PanelId });
    }

    [HttpGet]
    public IActionResult DrawResults(int panelId)
    {
        var selectedSubject = TempData["SelectedSubject"] as string ?? "Test";
        var selectedMessage = TempData["SelectedMessage"] as string ?? "Test";
        var reserveSubject = TempData["ReserveSubject"] as string ?? "Test";
        var reserveMessage = TempData["ReserveMessage"] as string ?? "Test";
        var notSelectedSubject = TempData["NotSelectedSubject"] as string ?? "Test";
        var notSelectedMessage = TempData["NotSelectedMessage"] as string ?? "Test";

        var panel = panelManager.GetPanelByIdWithInvitations(panelId);

        var criteria = panelManager.GetCriteriaByPanelIdWithSubcriteria(panelId);
        var criteriaNamesLookup = new Dictionary<int, string>();
        foreach(var criterion in criteria)
        {
            foreach(var subcriterion in criterion.SubCriteria)
            {
                if (!criteriaNamesLookup.ContainsKey(subcriterion.Id))
                {
                    criteriaNamesLookup[subcriterion.Id] = subcriterion.Name;
                }
            }
        }
        ViewBag.CriteriaNames = criteriaNamesLookup;
        
        var dr = panel.DrawResult;

        if (selectedSubject != "Test")
        {
            foreach (var selected in dr.SelectedInvitations)
            {
                mailSender.SendEmailAsync(selected.Email, selectedSubject,
                    selectedMessage.Replace(Environment.NewLine, "<br />"));
            }
        
            foreach (var reserve in dr.ReserveInvitations)
            {
                mailSender.SendEmailAsync(reserve.Email, reserveSubject,
                    reserveMessage.Replace(Environment.NewLine, "<br />"));
            }
        
            if (dr.NotSelectedInvitations.Count > 0)
            {
                foreach (var notSelected in dr.NotSelectedInvitations.ToList())
                {
                    mailSender.SendEmailAsync(notSelected.Email, notSelectedSubject,
                        notSelectedMessage.Replace(Environment.NewLine, "<br />"));
                    drawManager.RemoveInvitationByEmail(notSelected.Email);
                }
            }
        }
        
        foreach (var selected in dr.SelectedInvitations)
        {
            if (selected.UserId is not null)
            {
                var user = userProfileManager.GetUserByIdWithProfile(selected.UserId);
                panel.Members.Add(user.MemberProfile);
            }
        }
        panelManager.EditPanel(panel);

        ViewBag.PanelName = panel.Name;
        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;
        return View(dr);
    }

    [HttpGet]
    public IActionResult ReserveMembers(int panelId)
    {
        var model = new ReserveMailModel()
        {
            PanelId = panelId
        };
        return View(model);
    }
    
    [HttpPost]
    public IActionResult SendInvitations(ReserveMailModel model)
    {
        if (!ModelState.IsValid)
        {
            return View("ReserveMembers", model);
        }

        var invitationIds = JsonConvert.DeserializeObject<List<int>>(model.SelectedInvitationIds);
        var panel = panelManager.GetPanelByIdWithInvitations(model.PanelId);


        foreach (var invitationId in invitationIds)
        {
            var inv = drawManager.GetInvitationById(invitationId);
            if (inv != null)
            {
                mailSender.SendEmailAsync(inv.Email, model.Subject,
                    model.Message.Replace(Environment.NewLine, "<br />"));
                panel.DrawResult.ReserveInvitations.Remove(inv);
                panel.DrawResult.SelectedInvitations.Add(inv);
                inv.IsDrawn = true;
                drawManager.EditInvitation(inv);
            }
        }
        panelManager.EditPanel(panel);
        
        return RedirectToAction("Index", "Panel", new { id = model.PanelId });
    }
}