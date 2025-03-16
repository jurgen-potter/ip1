using CitizenPanel.BL;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;
using UI_MVC.Models.DTO;
using CitizenPanel.UI.MVC.Models;

namespace CitizenPanel.UI.MVC.Controllers;

using BL.Domain.Recruitment;
using Newtonsoft.Json;

public class MemberRegisterController : Controller
{
    private readonly IPanelUserManager _panelUserManager;
    private readonly IPanelManager _panelManager;

    public MemberRegisterController(IPanelUserManager panelUserManager, IPanelManager panelManager)
    {
        _panelUserManager = panelUserManager;
        _panelManager = panelManager;
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
         
        List<ExtraCriteria> criteriaList = data.CriteriaList;
        ViewBag.CriteriaList = criteriaList;
        
        var model = new NewMemberViewModel
        {
            Gender = data.Gender,
            Town = data.Town,
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
        
        var (result, member) = await _panelUserManager.AddMemberAsync(newMember.FirstName, newMember.LastName, newMember.Email, newMember.Password, newMember.Gender, newMember.BirthDate, newMember.Town, newMember.SelectedCriteria);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("Password", error.Description);
            }
            return View(newMember);
        }
        
        return RedirectToAction("Index", "Home");
    }
}