using CitizenPanel.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CitizenPanel.UI.MVC.Controllers;

using BL.Registration;
using Models;

public class RegistrationController(IRegistrationManager registrationManager,IEmailSender mailSender, IPanelManager panelManager,IDrawManager drawManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId)
    {
        var criteria = panelManager.GetCriteriaAndSubcriteriaWithPanelId(panelId);
        var panel    = panelManager.GetPanelById(panelId);
        var result   = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);

        var vm = new ResultViewModel {
            TotalNeededPanelmembers   = result.TotalNeededPanelmembers,
            ReservePotPanelmembers    = result.ReservePotPanelmembers,
            Buckets = result.Buckets.Select(b => new BucketViewModel {
                CriteriaNames     = b.CriteriaNames,
                SubCriteriaNames  = b.SubCriteriaNames,
                Count             = b.Count,
                RegisteredCount   = registrationManager
                    .GetBucketRegistrationsCount(panelId, b.CriteriaNames, b.SubCriteriaNames)
            }).ToList()
        };

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

        // registrationManager.StartFinalDraw(panel);
        
        
        TempData["SelectedSubject"] = finalDraw.SelectedSubject;
        TempData["SelectedMessage"] = finalDraw.SelectedMessage;
        TempData["ReserveSubject"] = finalDraw.ReserveSubject;
        TempData["ReserveMessage"] = finalDraw.ReserveMessage;
    
        // Always redirect to results
        return RedirectToAction("DrawResults", new { finalDraw.PanelId });
    }

    [HttpGet]
    public IActionResult DrawResults(int panelId)
    {
        // Create a panel object that matches the one in the RegistrationManager
        var panel = panelManager.GetPanelById(panelId);
    
        // Get draw status
        var drawResults = panel.DrawResult;

        var selectedSubject = TempData["SelectedSubject"] as string;
        var selectedMessage = TempData["SelectedMessage"] as string;
        var reserveSubject = TempData["ReserveSubject"] as string;
        var reserveMessage = TempData["ReserveMessage"] as string;
        
        
        
        foreach (var selected in drawResults.SelectedMembers)
        {
            mailSender.SendEmailAsync(selected.Email, selectedSubject, selectedMessage);
        }
        
        foreach (var reserve in drawResults.ReserveMembers)
        {
            mailSender.SendEmailAsync(reserve.Email, reserveSubject, reserveMessage);
        }

        foreach (var notSelected in drawResults.NotSelectedMembers)
        {
            mailSender.SendEmailAsync(notSelected.Email, "unlucky", "better luck next time");
        }
    
        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;
    
        return View(drawResults);
    }
}
