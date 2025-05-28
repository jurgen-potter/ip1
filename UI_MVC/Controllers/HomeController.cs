using System.Diagnostics;
using CitizenPanel.BL.Panels;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;

namespace CitizenPanel.UI.MVC.Controllers;

public class HomeController(IPanelManager panelManager) : Controller
{

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        var panels = panelManager.GetThreeActivePanels();
        var viewModel = new PanelSelectViewModel
        {
            Panels = panels.Select(p => new PanelSelectOptionViewModel
            {
                Id = p.Id,
                Name = p.Name,
                CoverImagePath = p.CoverImagePath,
                TenantId = p.TenantId
                
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Information()
    {
        return View();
    }
    
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult EditInformation()
    {
        return View();
    }
}