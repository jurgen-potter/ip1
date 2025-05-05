using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Users;

public class OrganizationController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}