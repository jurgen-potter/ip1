using CitizenPanel.BL;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class QRCodeController : Controller
{
    private readonly IUserManager _userManager;

    public QRCodeController(IUserManager userManager)
    {
        _userManager = userManager;
    }
    
    // GET
    public IActionResult Index(string code)
    {
        Panelmember pm = _userManager.AddPanelmember(code,"frank@kdg.be");
        return View(pm);
    }
}