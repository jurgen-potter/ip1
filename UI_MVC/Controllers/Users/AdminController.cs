using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.QuestionnaireModules;
using CitizenPanel.UI.MVC.Models;
using CitizenPanel.UI.MVC.Models.QuestionnaireModules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Users;

[Authorize(Roles = "Admin")]
public class AdminController(IQuestionnaireModuleManager questionnaireModuleManager) : Controller
{
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Index()
    {
        var questionnaires = questionnaireModuleManager.GetAllQuestionnaires();
        var questionnaireSelect = new QuestionnaireSelectViewModel();
        foreach (var questionnaire in questionnaires)
        {
            var questionnaireModel = new QuestionnaireViewModel()
            {
                Id = questionnaire.Id,
                Title = questionnaire.Title
            };
            questionnaireSelect.Questionnaires.Add(questionnaireModel);
        }
        return View(questionnaireSelect);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult EditQuestionnaire(int questionnaireId)
    {
        Questionnaire questionnaire = questionnaireModuleManager.GetQuestionnaire(questionnaireId);

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
                //Weight = question.Weight,
                IsDetail = question.IsDetail,
                ToDelete = false
            };
            foreach (Answer answer in question.Answers)
            {
                EditAnswerViewModel answerModel = new EditAnswerViewModel
                {
                    Id = answer.Id,
                    Description = answer.Description,
                    Advice = answer.Advice,
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
    [Authorize(Roles = "Admin")]
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
        
        var questionnaire = questionnaireModuleManager.GetQuestionnaire(model.Id);
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
                    //existingQuestion.Weight = questionModel.Weight;
                    existingQuestion.Position = questionPosition++;
                    existingQuestion.IsDetail = questionModel.IsDetail;

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
                                existingAnswer.Advice = answerModel.Advice;
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
                                Advice = answerModel.Advice,
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
                    //Weight = questionModel.Weight,
                    Position = questionPosition++,
                    IsDetail = questionModel.IsDetail,
                    Answers = questionModel.Answers.Select(a => new Answer
                    {
                        Description = a.Description,
                        Advice = a.Advice,
                        Position = answerPosition++,
                        IsCritical = a.IsCritical
                    }).ToList()
                };
                updatedQuestions.Add(newQuestion);
            }
        }

        questionnaire.Questions = updatedQuestions;

        questionnaireModuleManager.EditQuestionnaire(questionnaire);

        return RedirectToAction(nameof(Index));
    }
}