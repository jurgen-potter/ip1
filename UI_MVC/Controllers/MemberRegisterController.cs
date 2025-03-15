using CitizenPanel.BL;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;
using UI_MVC.Models.DTO;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Controllers;

public class MemberRegisterController : Controller
{
    private readonly IPanelUserManager _panelUserManager;

    public MemberRegisterController(IPanelUserManager panelUserManager)
    {
        _panelUserManager = panelUserManager;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Add(PanelmemberDTO panelmemberDto)
    {
        Panelmember Panelmember = _panelUserManager.AddPanelmember(panelmemberDto.Code,panelmemberDto.Email);
        return View(Panelmember);
    }
    
    [HttpGet]
    public IActionResult RegisterMember(PanelmemberDTO panelmemberDto)
    {
        Panelmember panelmember = _panelUserManager.AddPanelmember(panelmemberDto.Code,panelmemberDto.Email);

        var model = new NewMemberViewModel
        {
            Gender = panelmember.Gender,
            Town = panelmember.Postcode
        };
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterMember(NewMemberViewModel newMember)
    {
        if (!ModelState.IsValid) {
            return View(newMember);
        }
        
        var member = await _panelUserManager.AddMemberAsync(newMember.FirstName, newMember.LastName, newMember.Email, newMember.Password, newMember.Gender, newMember.BirthDate, newMember.Town, newMember.SelectedCriteria);
        return RedirectToAction("Index", "Home");
    }
}