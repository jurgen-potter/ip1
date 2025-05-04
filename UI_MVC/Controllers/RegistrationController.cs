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
        var members = memberManager.GetMembersOfPanelWithCriteria(panelId).ToList();
        var panel = panelManager.GetPanelById(panelId);
        var result = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);

        var bucketsWithActuals = registrationManager.AssignActualRegistrationsToBuckets(result.Buckets, members);

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

        var panel = panelManager.GetPanelById(finalDraw.PanelId);

        // Voer de loting uit
        registrationManager.StartFinalDraw(panel);

        TempData["SelectedSubject"] = finalDraw.SelectedSubject;
        TempData["SelectedMessage"] = finalDraw.SelectedMessage;
        TempData["ReserveSubject"] = finalDraw.ReserveSubject;
        TempData["ReserveMessage"] = finalDraw.ReserveMessage;
        TempData["NotSelectedSubject"] = "Niet geselecteerd voor het panel";
        TempData["NotSelectedMessage"] =
            "Helaas bent u niet geselecteerd voor deelname aan het panel. Bedankt voor uw interesse.";

        // Altijd doorverwijzen naar resultaten
        return RedirectToAction("DrawResults", new { finalDraw.PanelId });
    }

    [HttpGet]
    public IActionResult DrawResults(int panelId)
    {
        // Haal panel object op
        var panel = panelManager.GetPanelById(panelId);

        // Haal lotingsresultaat op
        var drawResults = panel.DrawResult;

        var selectedSubject = TempData["SelectedSubject"] as string;
        var selectedMessage = TempData["SelectedMessage"] as string;
        var reserveSubject = TempData["ReserveSubject"] as string;
        var reserveMessage = TempData["ReserveMessage"] as string;
        var notSelectedSubject = TempData["NotSelectedSubject"] as string;
        var notSelectedMessage = TempData["NotSelectedMessage"] as string;

        // // Verstuur e-mails naar geselecteerde deelnemers
        // foreach (var selected in drawResults.SelectedMembers)
        // {
        //     mailSender.SendEmailAsync(selected.Email, selectedSubject, selectedMessage);
        // }
        //
        // // Verstuur e-mails naar reserve deelnemers
        // foreach (var reserve in drawResults.ReserveMembers)
        // {
        //     mailSender.SendEmailAsync(reserve.Email, reserveSubject, reserveMessage);
        // }
        //
        // // Verstuur e-mails naar niet-geselecteerde deelnemers
        // foreach (var notSelected in drawResults.NotSelectedMembers)
        // {
        //     mailSender.SendEmailAsync(notSelected.Email, notSelectedSubject, notSelectedMessage);
        // }

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;

        return View(drawResults);
    }
}