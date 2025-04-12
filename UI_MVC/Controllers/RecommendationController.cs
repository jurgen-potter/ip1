using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Registration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CitizenPanel.UI.MVC.Controllers;

public class RecommendationController(IPanelManager panelManager) : Controller
{
    
    [HttpGet]
    public IActionResult Index(int panelId = 1) // momenteel nog niet gelinked aan een panel/account
    {
        var panel = panelManager.getPanelByIdWithRecommendations(panelId);
        var recommendations = panel.Recommendations;
        
        return View(recommendations);
    }
    
    [HttpPost]
    [HttpPost]
    public IActionResult Vote([FromBody] int id)
    {
        var recommendation = panelManager.GetRecommendationById(id);
        if (recommendation == null)
        {
            return NotFound();
        }

        recommendation.Votes++;
        panelManager.editRecommendation(recommendation);

        // Retourneer het nieuwe aantal stemmen
        return Json(new { id = recommendation.Id, votes = recommendation.Votes });
    }
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult AddRecommendation(Recommendation recommendation, Panel panel)
    {
        if (ModelState.IsValid)
        {
            panelManager.addRecommendationOfPanel(recommendation, panel);
            return RedirectToAction(nameof(Index));
        }
        return View(recommendation);
    }
}
