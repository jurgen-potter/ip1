using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.PanelManagement;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

using Models;

public class RegistrationController : Controller
{
    private readonly IRegistrationManager _registrationManager;
    private readonly IMailSender _mailSender;

    public RegistrationController(IRegistrationManager registrationManager, IMailSender mailSender)
    {
        _registrationManager = registrationManager;
        _mailSender = mailSender;
    }

    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        var panel = new Panel { PanelId = panelId };

        var allBuckets = _registrationManager.GetAllBuckets(panel);
        var drawStatus = _registrationManager.GetDrawStatus(panel);

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = drawStatus;
        ViewBag.HasSufficientRegistrations = _registrationManager.HasSufficientRegistrations(panel);

        return View(allBuckets);
    }
    
    [HttpGet]
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
        
        var panel = new Panel { PanelId = finalDraw.PanelId };

        // Always proceed with the draw regardless of sufficient registrations
        _registrationManager.StartFinalDraw(panel);
    
        // Always set success message
        TempData.Remove("ErrorMessage");
        TempData["SuccessMessage"] = "De loting is succesvol afgerond.";
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
        var panel = new Panel { PanelId = panelId, Name = $"Panel {panelId}" };
    
        // Get draw status
        var drawStatus = _registrationManager.GetDrawStatus(panel);
    
        // Get draw results
        var drawResults = _registrationManager.GetDrawResults(panel);
        
        var selectedSubject = TempData["SelectedSubject"] as string;
        var selectedMessage = TempData["SelectedMessage"] as string;
        var reserveSubject = TempData["ReserveSubject"] as string;
        var reserveMessage = TempData["ReserveMessage"] as string;
        foreach (var selected in drawResults.SelectedMembers)
        {
            _mailSender.SendMailAsync(selected.Email, selectedSubject, selectedMessage);
        }
        
        foreach (var reserve in drawResults.ReserveMembers)
        {
            _mailSender.SendMailAsync(reserve.Email, reserveSubject, reserveMessage);
        }
    
        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = drawStatus;
    
        return View(drawResults);
    }
}
