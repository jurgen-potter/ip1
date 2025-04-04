using CitizenPanel.BL;
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
    public IActionResult Index()
    {
        return View(_panelManager.GetPanel(1));
    }

    [Authorize(Roles = "Organization,Admin")]
    [HttpGet]
    public IActionResult MakePanel()
    {
        return View();
    }
}