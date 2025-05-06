using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Users;

public class OrganizationController : Controller
{
    [HttpGet]
    [Authorize (Roles = "Organization")]
    public IActionResult Index()
    {
        return View();
    }
}