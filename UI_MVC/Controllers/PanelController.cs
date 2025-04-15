using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Panel;
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
    public IActionResult Index(int? panelId)
    {
        if (panelId == null)
        {
            panelId = 1;
        }
        return View(_panelManager.GetPanel(panelId?? 1));
    }

    [Authorize(Roles = "Organization,Admin")]
    [HttpGet]
    public IActionResult MakePanel()
    {
        return View();
    }
    
    [Authorize(Roles = "Organization,Admin")]
    [HttpPost]
    public IActionResult MakePanel(Panel panel)
    {
        if(!ModelState.IsValid)
            return View();

        
        Panel newPanel = _panelManager.AddPanel(panel.Name,panel.Description,panel.EndDate);
        
        return RedirectToAction("Index","Panel",new {panelId=newPanel.PanelId});
    }

}