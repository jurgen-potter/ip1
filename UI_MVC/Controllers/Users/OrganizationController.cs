using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Users;

[Authorize (Roles = "Organization")]
public class OrganizationController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    public IActionResult ManageOrganization()
    {
        return View();
    }
    
    public IActionResult AddMember(int panelId)
    {
        return View(panelId);
    }
}