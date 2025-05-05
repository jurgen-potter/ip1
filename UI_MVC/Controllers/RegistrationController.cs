using CitizenPanel.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CitizenPanel.UI.MVC.Controllers;

using BL.Registration;
using Models;

public class RegistrationController(
    IRegistrationManager registrationManager,
    IEmailSender mailSender,
    IPanelManager panelManager,
    IDrawManager drawManager,
    IMemberManager memberManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId)
    {
        var criteria = panelManager.GetCriteriaAndSubcriteriaWithPanelId(panelId);
        var users = memberManager.GetMembersOfPanelWithCriteria(panelId).ToList();
        var panel = panelManager.GetPanelById(panelId);
        var result = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);

        var bucketsWithActuals = registrationManager.AssignActualRegistrationsToBuckets(result.Buckets, users);

        var vm = new ResultViewModel
        {
            TotalAvailablePotentialPanelmembers = panel.TotalAvailablePotentialPanelmembers,
            ReservePotPanelmembers = result.ReservePotPanelmembers,
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

        var panel = panelManager.GetPanelByIdWithMembers(finalDraw.PanelId);
        registrationManager.StartFinalDraw(panel);

        TempData["SelectedSubject"] = finalDraw.SelectedSubject;
        TempData["SelectedMessage"] = finalDraw.SelectedMessage;
        TempData["ReserveSubject"] = finalDraw.ReserveSubject;
        TempData["ReserveMessage"] = finalDraw.ReserveMessage;
        TempData["NotSelectedSubject"] = "Niet geselecteerd voor het panel";
        TempData["NotSelectedMessage"] =
            "Helaas bent u niet geselecteerd voor deelname aan het panel. Bedankt voor uw interesse.";


        return RedirectToAction(nameof(DrawResults), new { panelId = finalDraw.PanelId });
    }

    [HttpGet]
    public IActionResult DrawResults(int panelId)
    {
        var selectedSubject = TempData["SelectedSubject"] as string;
        var selectedMessage = TempData["SelectedMessage"] as string;
        var reserveSubject = TempData["ReserveSubject"] as string;
        var reserveMessage = TempData["ReserveMessage"] as string;
        var notSelectedSubject = TempData["NotSelectedSubject"] as string;
        var notSelectedMessage = TempData["NotSelectedMessage"] as string;

        var panel = panelManager.GetPanelByIdWithMembers(panelId);

        var dr = panel.DrawResult;


        foreach (var selected in dr.SelectedMembers)
        {
            mailSender.SendEmailAsync(selected.Email, selectedSubject, selectedMessage);
        }

        foreach (var reserve in dr.ReserveMembers)
        {
            mailSender.SendEmailAsync(reserve.Email, reserveSubject, reserveMessage);
        }

        foreach (var notSelected in dr.NotSelectedMembers)
        {
            mailSender.SendEmailAsync(notSelected.Email, notSelectedSubject, notSelectedMessage);
            drawManager.RemoveInvitationByEmail(notSelected.Email);
        }

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;
        return View(dr);
    }
}