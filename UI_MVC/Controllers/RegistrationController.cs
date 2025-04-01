using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

using Models;

public class RegistrationController : Controller
{
    private readonly IRegistrationManager _registrationManager;
    private readonly IMailSender _mailSender;
    private readonly IPanelManager _panelManager;

    public RegistrationController(IRegistrationManager registrationManager, IMailSender mailSender, IPanelManager panelManager)
    {
        _registrationManager = registrationManager;
        _mailSender = mailSender;
        _panelManager = panelManager;
    }

    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        var panel = _panelManager.GetPanel(panelId);

        var allBuckets = _registrationManager.GetAllBuckets(panel);

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = panel.DrawStatus;
        ViewBag.HasSufficientRegistrations = _registrationManager.HasSufficientRegistrations(panel);

        return View(allBuckets);
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
        
        var panel = _panelManager.GetPanel(finalDraw.PanelId);

        // Always proceed with the draw regardless of sufficient registrations
        _registrationManager.StartFinalDraw(panel);
        
    
       
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
        var panel = _panelManager.GetPanel(panelId);
    
        // Get draw status
        var drawResults = _registrationManager.getDrawResults(panel);
        
        // Get draw results
        var selectedSubject = TempData["SelectedSubject"] as string;
        var selectedMessage = TempData["SelectedMessage"] as string;
        var reserveSubject = TempData["ReserveSubject"] as string;
        var reserveMessage = TempData["ReserveMessage"] as string;
        
        foreach (var selected in drawResults.SelectedMembers)
        {
            _mailSender.SendMailAsync("donaldduckie313@gmail.com", selectedSubject, selectedMessage);
        }
        
        foreach (var reserve in drawResults.ReserveMembers)
        {
            _mailSender.SendMailAsync("donaldduckie313@gmail.com", reserveSubject, reserveMessage);
        }
    
        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = drawStatus;
    
        return View(drawResults);
    }
}
