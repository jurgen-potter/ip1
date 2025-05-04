using System.Security.Claims;
using CitizenPanel.BL;
using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.BL.QuestionnaireModule;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class QuestionnaireController : Controller
{
    private readonly IQuestionnaireModuleManager _questionnaireModuleManager;
    private readonly IMemberManager _memberManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public QuestionnaireController(IQuestionnaireModuleManager questionnaireModuleManager, IMemberManager memberManager, UserManager<ApplicationUser> userManager)
    {
        _questionnaireModuleManager = questionnaireModuleManager;
        _memberManager = memberManager;
        _userManager = userManager;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int questionnaireId)
    {
        var questionnaire = _questionnaireModuleManager.GetQuestionnaire(questionnaireId);
        var user = await _userManager.GetUserAsync(User);

        List<int> selectedAnswerIds = new();
        if (User.IsInRole("Organization") && user?.OrganizationProfile?.Answers != null)
        {
            selectedAnswerIds = user.OrganizationProfile.Answers.Select(a => a.Id).ToList();
        }

        var viewModel = new QuestionnaireResponseViewModel()
        {
            Questionnaire = questionnaire,
            QuestionnaireId = questionnaireId,
            SelectedAnswerIds = selectedAnswerIds
        };

        return View(viewModel);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult Result(QuestionnaireResponseViewModel model)
    {
        model.Questionnaire = _questionnaireModuleManager.GetQuestionnaire(model.QuestionnaireId);
        
        var answers = new List<Answer>();
        foreach (var answer in model.Answers)
        {
            answers.Add(_questionnaireModuleManager.GetAnswer(answer.Value));
        }

        model.IsCritical = answers.Any(a => a.IsCritical);
        
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Organization")]
    public async Task<IActionResult> Save(QuestionnaireResponseViewModel model)
    {
        if (!User.IsInRole("Organization"))
        {
            return Forbid();
        }
        
        ModelState.Clear();
        
        var answers = new List<Answer>();
        foreach (var answer in model.Answers)
        {
            answers.Add(_questionnaireModuleManager.GetAnswer(answer.Value));
        }
        var user = await _userManager.GetUserAsync(User);
        await _memberManager.ChangeOrganizationAnswersAsync(user.Id, answers);
        return RedirectToAction(nameof(Index), new { questionnaireId = model.QuestionnaireId });
    }
}