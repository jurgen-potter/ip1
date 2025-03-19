using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.PanelManagement;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class RegistrationController : Controller
{
    private readonly IRegistrationManager registrationManager;

    public RegistrationController(IRegistrationManager registrationManager)
    {
        this.registrationManager = registrationManager;
    }

    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        var panel = new Panel { PanelId = panelId };

        var allBuckets = registrationManager.GetAllBuckets(panel);
        var drawStatus = registrationManager.GetDrawStatus(panel);

        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = drawStatus;
        ViewBag.HasSufficientRegistrations = registrationManager.HasSufficientRegistrations(panel);

        return View(allBuckets);
    }

    [HttpPost]
    public IActionResult StartFinalDrawPhase(int panelId)
    {
        var panel = new Panel { PanelId = panelId };

        // Always proceed with the draw regardless of sufficient registrations
        registrationManager.StartFinalDraw(panel);
    
        // Always set success message
        TempData.Remove("ErrorMessage");
        TempData["SuccessMessage"] = "De loting is succesvol afgerond.";
    
        // Always redirect to results
        return RedirectToAction("DrawResults", new { panelId });
    }

    [HttpGet]
    public IActionResult DrawResults(int panelId)
    {
        // Create a panel object that matches the one in the RegistrationManager
        var panel = new Panel { PanelId = panelId, Name = $"Panel {panelId}" };
    
        // Get draw status
        var drawStatus = registrationManager.GetDrawStatus(panel);
    
        // Get draw results
        var drawResults = registrationManager.GetDrawResults(panel);
    
        ViewBag.PanelId = panelId;
        ViewBag.DrawStatus = drawStatus;
    
        return View(drawResults);
    }
}
