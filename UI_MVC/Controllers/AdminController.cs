using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.QuestionnaireModule;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

[Authorize (Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IQuestionnaireModuleManager _questionnaireModuleManager;

    public AdminController(IQuestionnaireModuleManager questionnaireModuleManager)
    {
        _questionnaireModuleManager = questionnaireModuleManager;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult EditQuestionnaire(int questionnaireId = 1)
    {
        Questionnaire questionnaire = _questionnaireModuleManager.GetQuestionnaire(questionnaireId);
        
        EditQuestionnaireViewModel model = new EditQuestionnaireViewModel
        {
            Id = questionnaire.Id,
            Title = questionnaire.Title
        };
        foreach (Question question in questionnaire.Questions)
        {
            EditQuestionViewModel questionModel = new EditQuestionViewModel
            {
                Id = question.Id,
                Description = question.Description,
                Weight = question.Weight
            };
            foreach (Answer answer in question.Answers)
            {
                EditAnswerViewModel answerModel = new EditAnswerViewModel
                {
                    Id = answer.Id,
                    Description = answer.Description
                };
                questionModel.Answers.Add(answerModel);
            }
            model.Questions.Add(questionModel);
        }
        
        return View(model);
    }
    
    [HttpPost]
    public IActionResult EditQuestionnaire(EditQuestionnaireViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        return RedirectToAction(nameof(Index));
    }
}