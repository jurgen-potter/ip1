using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.QuestionnaireModules;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.QuestionnaireModules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CitizenPanel.UI.MVC.Controllers.QuestionnaireModules;

public class QuestionnaireController(
    IQuestionnaireModuleManager questionnaireModuleManager,
    IMemberManager memberManager,
    UserManager<ApplicationUser> userManager) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int questionnaireId)
    {
        var questionnaire = questionnaireModuleManager.GetQuestionnaireById(questionnaireId);
        var userId = userManager.GetUserId(User);
        var organization = memberManager.GetOrganizationByIdWithAnswers(userId);
        List<int> selectedAnswerIds = new();

        if (User.IsInRole("Organization") && organization?.OrganizationProfile?.Answers != null)
        {
            selectedAnswerIds = organization.OrganizationProfile.Answers
                .Where(a => a.Question != null && a.Question.Questionnaire?.Id == questionnaireId)
                .Select(a => a.Id).ToList();
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
        model.Questionnaire = questionnaireModuleManager.GetQuestionnaireById(model.QuestionnaireId);
        
        var answers = new List<Answer>();
        foreach (var answer in model.Answers)
        {
            answers.Add(questionnaireModuleManager.GetAnswerById(answer.Value));
        }

        model.IsCritical = answers.Any(a => a.IsCritical);
        
        return View(model);
    }

    [HttpPost]
    [AllowAnonymous]
    public IActionResult PrepareSave(QuestionnaireResponseViewModel model)
    {
        if (!ModelState.IsValid)
            return View("Index", model);

        TempData["FormData"] = JsonConvert.SerializeObject(model);

        return RedirectToAction("Save");
    }

    
    [HttpGet]
    [Authorize(Roles = "Organization")]
    public async Task<IActionResult> Save()
    {
        var json = TempData["FormData"].ToString();
        var model = JsonConvert.DeserializeObject<QuestionnaireResponseViewModel>(json);
        
        var answers = new List<Answer>();
        foreach (var answer in model.Answers)
        {
            answers.Add(questionnaireModuleManager.GetAnswerById(answer.Value));
        }
        
        var user = await userManager.GetUserAsync(User);
        await memberManager.ChangeOrganizationAnswersAsync(user.Id, model.QuestionnaireId, answers);
        return RedirectToAction(nameof(Index), new { questionnaireId = model.QuestionnaireId });
    }
}