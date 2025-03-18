using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Recruitment;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class MemberRegisterController : Controller
{
    private readonly IRegistrationManager _registrationManager;
    private readonly IPanelManager _panelManager;
    private readonly IDrawManager _drawManager;

    public MemberRegisterController(IRegistrationManager registrationManager, IPanelManager panelManager, IDrawManager drawManager)
    {
        _registrationManager = registrationManager;
        _panelManager = panelManager;
        _drawManager = drawManager;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
    
    [HttpGet]
    public IActionResult InvalidCode()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult UsedCode()
    {
        return View();
    }

    
    [HttpGet]
    public IActionResult RegisterMember(MemberDto memberDto)
    {
        Invitation invitation = memberDto.Invitation;
        List<ExtraCriteria> extraCriteria = memberDto.CriteriaList;
        if (invitation == null)
        {
            invitation = _drawManager.GetInvitationWithCode(memberDto.Code);
        
            if (invitation == null)
                return RedirectToAction("InvalidCode", "MemberRegister");

            if (invitation.IsUsed)
                return RedirectToAction("UsedCode", "MemberRegister");
            
            extraCriteria = _panelManager.GetAllExtraCriteria();
        }


        var model = new NewMemberViewModel
        {
            Gender = invitation.Gender,
            Town = invitation.Postcode,
            CriteriaList = extraCriteria,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            PanelId = 1,
            Invitation = invitation,
            IsConfirmed = memberDto.IsConfirmed
        };
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterMember(NewMemberViewModel newMember)
    {
        if (!ModelState.IsValid) {
            return View(newMember);
        }
        
        var (result, member) = await _registrationManager.AddMemberAsync(newMember.FirstName, newMember.LastName, newMember.Email, newMember.Password, newMember.Gender, newMember.BirthDate, newMember.Town, newMember.SelectedCriteria, newMember.PanelId);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }
            return View(newMember);
        }
        newMember.Invitation.IsUsed = true;
        _drawManager.ChangeInvitation(newMember.Invitation);
        return RedirectToAction("RegistrationConfirmed");
    }

    public IActionResult RegistrationConfirmed()
    {
        return View();
    }
}