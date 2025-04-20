using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class PanelController : Controller
{
    private readonly IPanelManager _panelManager;

    public PanelController(IPanelManager panelManager)
    {
        _panelManager = panelManager;
    }
    
    // GET
    public IActionResult Index(int panelId = 1)
    {
        Panel panel = _panelManager.GetPanelById(panelId);

        PanelViewModel model = new PanelViewModel()
        {
            PanelId = panel.PanelId,
            Name = panel.Name,
            Description = panel.Description,
            StartDate = panel.StartDate,
            EndDate = panel.EndDate,
            CoverImagePath = panel.CoverImagePath
        };
        
        return View(model);
    }

    [Authorize(Roles = "Organization")]
    [HttpGet]
    public IActionResult CreatePanel()
    {
        CreatePanelViewModel model = new CreatePanelViewModel();
        return View(model);
    }
    
    [Authorize(Roles = "Organization")]
    [HttpPost]
    public IActionResult CreatePanel(CreatePanelViewModel model)
    {
        if(!ModelState.IsValid)
            return View(model);

        ICollection<ExtraCriteria> criteria = TempData["Criteria"] as ICollection<ExtraCriteria>  ?? new List<ExtraCriteria>();
        
        Panel newPanel = _panelManager.AddPanel(model.Name, model.Description, model.EndDate, criteria);
        
        return RedirectToAction("Index","Panel",new {panelId=newPanel.PanelId});
    }

}