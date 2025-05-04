using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class OrganizationController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}