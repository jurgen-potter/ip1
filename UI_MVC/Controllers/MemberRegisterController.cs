using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;

namespace CitizenPanel.UI.MVC.Controllers;

public class MemberRegisterController : Controller
{
    private readonly IDrawManager _drawManager;
    private readonly IEmailSender _emailSender;
    private readonly IMemberManager _memberManager;
    private readonly IPanelManager _panelManager;

    public MemberRegisterController(IDrawManager drawManager, IEmailSender emailSender, IMemberManager memberManager, IPanelManager panelManager)
    {
        _emailSender = emailSender;
        _drawManager = drawManager;
        _memberManager = memberManager;
        _panelManager = panelManager;
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
    
    [HttpGet]
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
            
            if (invitation.IsDrawn)
                return RedirectToAction("UsedCode", "MemberRegister");
        }
        
        List<Criteria> extraCriteria = _panelManager.GetExtraCriteriaByPanelId(invitation.PanelId).ToList();

        var model = new RegisterViewModel()
        {
            Invitation = invitation,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            CriteriaList = extraCriteria,
            IsConfirmed = memberDto.IsConfirmed
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
        _emailSender.SendEmailAsync(email, "Bevestiging aanmelding", "Uw gegevens zijn opgeslagen");
        
        return View();
    }
}