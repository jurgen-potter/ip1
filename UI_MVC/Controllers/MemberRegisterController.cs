using CitizenPanel.BL;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Controllers;

using BL.Domain.Recruitment;
using Models.DTO;
using Newtonsoft.Json;

public class MemberRegisterController : Controller
{
    private readonly IPanelUserManager _panelUserManager;
    private readonly IPanelManager _panelManager;
    private readonly IRegistrationManager _registrationManager;
    private readonly IMailSender _mailSender;

    public MemberRegisterController(IPanelUserManager panelUserManager, IPanelManager panelManager, IRegistrationManager registrationManager, IMailSender mailSender)
    {
        _panelUserManager = panelUserManager;
        _panelManager = panelManager;
        _registrationManager = registrationManager;
        _mailSender = mailSender;
    }
    
    // GET
    public IActionResult Index()
    {
        return View();
    }

    /*public IActionResult Add(MemberDto memberDto)
    {
        Panelmember Panelmember = _panelUserManager.AddPanelmember(memberDto.Code,memberDto.Email);
        return View(Panelmember);
    }*/

    [HttpGet]
    public IActionResult JoinPanelConfirmation(MemberDto memberDto)
    {
        return View(memberDto);
    }
    
    [HttpGet]
    public IActionResult RegisterMember(MemberDto memberDto)
    {
        Panelmember panelMember = _panelUserManager.AddPanelmember(memberDto.Code,memberDto.Email);

        List<ExtraCriteria> extraCriteria = _panelManager.GetAllExtraCriteria();

        var model = new NewMemberViewModel
        {
            Gender = panelMember.Gender,
            Town = panelMember.Postcode,
            CriteriaList = extraCriteria,
            SelectedCriteria = new List<int>(new int[extraCriteria.Count]),
            PanelId = 1
        };
        
        /*CodeData data = _panelUserManager.GetCodeData(memberDto.Code);
        
        var model = new NewMemberViewModel
        {
            Gender = data.Gender,
            Town = data.Town,
            CriteriaList = data.CriteriaList,
            SelectedCriteria = new List<SubCriteria>(new SubCriteria[extraCriteria.Count]),
            PanelId = data.PanelId
        };*/
        
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