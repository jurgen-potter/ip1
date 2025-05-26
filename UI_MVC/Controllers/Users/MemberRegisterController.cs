using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.Draws;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Users;

public class MemberRegisterController(
    IDrawManager drawManager,
    IEmailSender emailSender,
    IPanelManager panelManager,
    UserManager<ApplicationUser> userManager) : Controller
{
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
        Invitation invitation = drawManager.GetInvitationByCode(code);
        
        if (invitation == null)
            return RedirectToAction("InvalidCode", "MemberRegister");

        if (invitation.IsRegistered)
            return RedirectToAction("Registered", "MemberRegister");
            
        if (invitation.IsDrawn)
            return RedirectToAction("UsedCode", "MemberRegister");
        
        List<Criteria> extraCriteria = panelManager.GetExtraCriteriaByPanelId(invitation.PanelId).ToList();
        
        var model = new RegisterViewModel()
        {
            Code = code,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            IsConfirmed = false
        };
        
        if (User.Identity is { IsAuthenticated: true })
        {
            var user = userManager.GetUserAsync(User).Result;
            model.Email = user?.Email;
        }

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
        ApplicationUser user = null;
        if (User.Identity is { IsAuthenticated: true })
        {
            user = userManager.GetUserAsync(User).Result;
        }
        
        if (newMember is IValidatableObject validatable && newMember.Email == null && user is null)
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
        
        Invitation invitation = drawManager.GetInvitationByCode(newMember.Code);
        invitation.SelectedCriteria = newMember.SelectedCriteria;
        invitation.IsRegistered = true;
        invitation.Email = newMember.Email;
        invitation.UserId = user?.Id;
        drawManager.EditInvitation(invitation);

        TempData["Email"] = newMember.Email;
        return RedirectToAction("RegistrationConfirmed");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult RegistrationConfirmed()
    {
        var email = TempData["Email"]?.ToString();
        emailSender.SendEmailAsync(email, "Bevestiging aanmelding", "Uw gegevens zijn opgeslagen");
        
        return View();
    }
}