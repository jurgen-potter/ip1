using System.Security.Claims;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;
[Authorize]
public class RecommendationController(IPanelManager panelManager) : Controller
{
    [HttpGet]
    public IActionResult Index(int panelId = 1)
    {
        var panel = panelManager.GetPanelByIdWithRecommendations(panelId);
        var recommendations = panel.Recommendations;
        
        return View(recommendations);
    }
    
    
    public IActionResult AddRecommendation()
    {
        return View();
    }

    
    [Authorize(Roles = "Organization")]
    [HttpPost]
    public IActionResult AddRecommendation(string title, string description)
    {
        var panel = panelManager.GetPanelById(1); //gehardcode omdat panels nog niet echt gelinked zijn aan gebruikers.
        if (ModelState.IsValid)
        {
            panelManager.AddRecommendationOfPanel(title, description, panel);
            return RedirectToAction(nameof(Index));
        }
        return View("Index");
    }
}