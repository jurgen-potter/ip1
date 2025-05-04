using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
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
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    

    [HttpGet]
    [AllowAnonymous]
    public IActionResult InvalidCode()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Registered()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult UsedCode()
    {
        return View();
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegisterMember(string code)
    {
        Invitation invitation = _drawManager.GetInvitationWithCode(code);
        
        if (invitation == null)
            return RedirectToAction("InvalidCode", "MemberRegister");

        if (invitation.IsRegistered)
            return RedirectToAction("Registered", "MemberRegister");
            
        if (invitation.IsDrawn)
            return RedirectToAction("UsedCode", "MemberRegister");
        
        List<Criteria> extraCriteria = _panelManager.GetExtraCriteriaByPanelId(invitation.PanelId).ToList();
        
        var model = new RegisterViewModel()
        {
            Code = code,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            IsConfirmed = false
        };

        foreach (var criteria in extraCriteria)
        {
            var criteriaModel = new CriteriaViewModel()
            {
                Name = criteria.Name
            };
            foreach (var subCriteria in criteria.SubCriteria)
            {
                var subCriteriaModel = new SubCriteriaViewModel()
                {
                    Id = subCriteria.Id,
                    Name = subCriteria.Name
                };
                criteriaModel.SubCriteria.Add(subCriteriaModel);
            }
            model.CriteriaList.Add(criteriaModel);
        }
        
        return View(model);
    }
    
    [HttpPost]
    [AllowAnonymous]
    public IActionResult RegisterMember(string code, RegisterViewModel newMember)
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
            newMember.IsConfirmed = true;
            return View(newMember);
        }
        
        Invitation invitation = _drawManager.GetInvitationWithCode(newMember.Code);
        invitation.SelectedCriteria = newMember.SelectedCriteria;
        invitation.IsRegistered = true;
        invitation.Email = newMember.Email;
        _drawManager.ChangeInvitation(invitation);

        TempData["Email"] = newMember.Email;
        return RedirectToAction("RegistrationConfirmed");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegistrationConfirmed()
    {
        var email = TempData["Email"]?.ToString();
        _emailSender.SendEmailAsync(email, "Bevestiging aanmelding", "Uw gegevens zijn opgeslagen");
        
        return View();
    }
}