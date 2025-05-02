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
                Weight = question.Weight,
                ToDelete = false
            };
            foreach (Answer answer in question.Answers)
            {
                EditAnswerViewModel answerModel = new EditAnswerViewModel
                {
                    Id = answer.Id,
                    Description = answer.Description,
                    IsCritical = answer.IsCritical,
                    ToDelete = false
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
        ModelState.Clear();
    
        // Clean up the data
        var existingQuestionsList = new List<EditQuestionViewModel>();
        foreach (EditQuestionViewModel question in model.Questions.ToList())
        {
            var existingAnswersList = new List<EditAnswerViewModel>();
            foreach (EditAnswerViewModel answer in question.Answers.ToList())
            {
                if (!answer.ToDelete)
                {
                    existingAnswersList.Add(answer);
                }
            }
            question.Answers = existingAnswersList;
            
            if (!question.ToDelete)
            {
                existingQuestionsList.Add(question);
            }
        }
        model.Questions = existingQuestionsList;
    
        TryValidateModel(model);
        
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
        int questionPosition = 1;

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
                                existingAnswer.IsCritical = answerModel.IsCritical;
                                updatedAnswers.Add(existingAnswer);
                            }
                        }
                        else
                        {
                            var newAnswer = new Answer
                            {
                                Description = answerModel.Description,
                                Position = answerPosition++,
                                IsCritical = answerModel.IsCritical
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
                int answerPosition = 0;
                var newQuestion = new Question
                {
                    Description = questionModel.Description,
                    Weight = questionModel.Weight,
                    Position = questionPosition++,
                    Answers = questionModel.Answers.Select(a => new Answer
                    {
                        Description = a.Description,
                        Position = answerPosition++,
                        IsCritical = a.IsCritical
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