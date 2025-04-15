using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.BL.QuestionnaireModule;
using CitizenPanel.UI.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

[Authorize(Roles = "Admin")]
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
        
        var questionnaire = _questionnaireModuleManager.GetQuestionnaire(model.Id);
        if (questionnaire == null)
        {
            return NotFound();
        }
        
        questionnaire.Title = model.Title;
        
        var updatedQuestions = new List<Question>();
        int questionPosition = 0;

        foreach (var questionModel in model.Questions)
        {
            if (questionModel.Id > 0)
            {
                var existingQuestion = questionnaire.Questions.SingleOrDefault(q => q.Id == questionModel.Id);
                if (existingQuestion != null)
                {
                    existingQuestion.Description = questionModel.Description;
                    existingQuestion.Weight = questionModel.Weight;
                    existingQuestion.Position = questionPosition++;

                    var updatedAnswers = new List<Answer>();
                    int answerPosition = 0;

                    foreach (var answerModel in questionModel.Answers)
                    {
                        if (answerModel.Id > 0)
                        {
                            var existingAnswer = existingQuestion.Answers.SingleOrDefault(a => a.Id == answerModel.Id);
                            if (existingAnswer != null)
                            {
                                existingAnswer.Description = answerModel.Description;
                                existingAnswer.Position = answerPosition++;
                                updatedAnswers.Add(existingAnswer);
                            }
                        }
                        else
                        {
                            var newAnswer = new Answer
                            {
                                Description = answerModel.Description,
                                Position = answerPosition++
                            };
                            updatedAnswers.Add(newAnswer);
                        }
                    }

                    existingQuestion.Answers = updatedAnswers;
                    updatedQuestions.Add(existingQuestion);
                }
            }
            else
            {
                var newQuestion = new Question
                {
                    Description = questionModel.Description,
                    Weight = questionModel.Weight,
                    Position = questionPosition++,
                    Answers = questionModel.Answers.Select(a => new Answer
                    {
                        Description = a.Description
                    }).ToList()
                };
                updatedQuestions.Add(newQuestion);
            }
        }

        questionnaire.Questions = updatedQuestions;

        _questionnaireModuleManager.EditQuestionnaire(questionnaire);

        return RedirectToAction(nameof(Index));
    }
}