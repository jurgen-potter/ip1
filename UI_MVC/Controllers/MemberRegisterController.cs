using System.Text.Json;
using System.Web;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CitizenPanel.UI.MVC.Controllers;

using BL.Domain.Recruitment;

public class MemberRegisterController : Controller
{
    private readonly IPanelUserManager _panelUserManager;
    private readonly IPanelManager _panelManager;
    private readonly IDrawManager _drawManager;

    public MemberRegisterController(IPanelUserManager panelUserManager, IPanelManager panelManager, IDrawManager drawManager)
    {
        _panelUserManager = panelUserManager;
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
        
        var (result, member) = await _panelUserManager.AddMemberAsync(newMember.FirstName, newMember.LastName, newMember.Email, newMember.Password, newMember.Gender, newMember.BirthDate, newMember.Town, newMember.SelectedCriteria);
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