using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class TenantTestingController : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
}