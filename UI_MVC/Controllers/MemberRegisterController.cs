using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class MemberRegisterController : Controller
{
    private readonly IDrawManager _drawManager;
    private readonly IMailSender _mailSender;
    private readonly IMemberManager _memberManager;

    public MemberRegisterController(IDrawManager drawManager, IMailSender mailSender, IMemberManager memberManager)
    {
        _mailSender = mailSender;
        _drawManager = drawManager;
        _memberManager = memberManager;
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
    public IActionResult Registered()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult UsedCode()
    {
        return View();
    }

    
    /*[HttpGet]
    public IActionResult RegisterMember(MemberDto memberDto)
    {
        Invitation invitation = memberDto.Invitation;
        if (invitation == null)
        {
            invitation = _drawManager.GetInvitationWithCode(memberDto.Code);
        
            if (invitation == null)
                return RedirectToAction("InvalidCode", "MemberRegister");

            if (invitation.IsRegistered)
                return RedirectToAction("Registered", "MemberRegister");
            
            if (invitation.IsUsed)
                return RedirectToAction("UsedCode", "MemberRegister");
        }
        
        List<ExtraCriteria> extraCriteria = _drawManager.GetExtraCriteriaByPanel(invitation.PanelId).ToList();

        var model = new RegisterViewModel()
        {
            Invitation = invitation,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            CriteriaList = extraCriteria,
            IsConfirmed = memberDto.IsConfirmed
            
        };
        /*
        var model = new NewMemberViewModel
        {
            Gender = invitation.Gender,
            Town = invitation.Postcode,
            CriteriaList = extraCriteria,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            PanelId = 1,
            Invitation = invitation,
            IsConfirmed = memberDto.IsConfirmed
        };*/
        
        return View(model);
    }*/
    
    [HttpGet]
    public IActionResult RegisterMember(string code)
    {
        Invitation invitation = _drawManager.GetInvitationWithCode(code);
        if (invitation.IsUsed)
            return RedirectToAction("UsedCode", "MemberRegister");
        
        List<ExtraCriteria> extraCriteria = _drawManager.GetExtraCriteriaByPanel(invitation.PanelId).ToList();

        var model = new NewMemberViewModel
        {
            Gender = invitation.Gender,
            Town = invitation.Postcode,
            CriteriaList = extraCriteria,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            PanelId = 1,
            Invitation = invitation,
            IsConfirmed = true
        };
        
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> RegisterMember(RegisterViewModel newMember)
    {
        if (newMember is IValidatableObject validatable && newMember.Email == null)
        {
            var context = new ValidationContext(newMember);
            var validationResults = validatable.Validate(context);

            foreach (var validationResult in validationResults)
            {
                foreach (var memberName in validationResult.MemberNames)
                {
                    if (!ModelState.ContainsKey(memberName) || ModelState[memberName].Errors.All(e => e.ErrorMessage != validationResult.ErrorMessage))
                    {
                        ModelState.AddModelError(memberName, validationResult.ErrorMessage);
                    }
                }
            }
        }
        if (!ModelState.IsValid) {
            return View(newMember);
        }
        
        /*
        var (result, member) = await _memberManager.AddMemberAsync(newMember.FirstName, newMember.LastName, newMember.Email, newMember.Password, newMember.Gender, newMember.BirthDate, newMember.Town, newMember.SelectedCriteria, newMember.PanelId);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }
            return View(newMember);
        }*/
        
        newMember.Invitation.SelectedCriteria = newMember.SelectedCriteria;
        newMember.Invitation.IsRegistered = true;
        newMember.Invitation.Email = newMember.Email;
        _drawManager.ChangeInvitation(newMember.Invitation);

        TempData["Email"] = newMember.Email;
        return RedirectToAction("RegistrationConfirmed");
    }

    public IActionResult RegistrationConfirmed()
    {
        var email = TempData["Email"]?.ToString();
        _mailSender.SendMailAsync(email, "Bevestiging aanmelding", "Uw gegevens zijn opgeslagen");
        
        return View();
    }
}