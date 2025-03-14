using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;
using UI_MVC.Models.DTO;

namespace UI_MVC.Controllers;

public class PanelmemberRegisterController : Controller
{
    private readonly IUserManager _userManager;

    public PanelmemberRegisterController(IUserManager userManager)
    {
        _userManager = userManager;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Add(PanelmemberDTO panelmemberDto)
    {
        Panelmember Panelmember = _userManager.AddPanelmember(panelmemberDto.Code,panelmemberDto.Email);
        return View(Panelmember);
    }
    
    
}