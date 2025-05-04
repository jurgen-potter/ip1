using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.QuestionnaireModule;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class QuestionnaireController : Controller
{
    private readonly IQuestionnaireModuleManager _questionnaireModuleManager;

    public QuestionnaireController(IQuestionnaireModuleManager questionnaireModuleManager)
    {
        _questionnaireModuleManager = questionnaireModuleManager;
    }
    
    [HttpGet]
    public IActionResult Index(int questionnaireId)
    {
        return View(_questionnaireModuleManager.GetQuestionnaire(questionnaireId));
    }

    [HttpPost]
    public IActionResult Result(QuestionnaireResponseViewModel questionnaireResponseViewModel)
    {
        questionnaireResponseViewModel.Questionnaire = _questionnaireModuleManager.GetQuestionnaire(questionnaireResponseViewModel.QuestionnaireId);
        
        var answers = new List<Answer>();
        foreach (var answer in questionnaireResponseViewModel.Answers)
        {
            answers.Add(_questionnaireModuleManager.GetAnswer(answer.Value));
        }

        questionnaireResponseViewModel.IsCritical = answers.Any(a => a.IsCritical);
        
        return View(questionnaireResponseViewModel);
    }
}