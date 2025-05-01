using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class TenantTestingController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}