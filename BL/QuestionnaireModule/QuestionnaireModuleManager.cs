using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.DAL.QuestionnaireModule;

namespace CitizenPanel.BL.QuestionnaireModule;

public class QuestionnaireModuleManager : IQuestionnaireModuleManager
{
    private readonly IQuestionnaireModuleRepository _repository;

    public QuestionnaireModuleManager(IQuestionnaireModuleRepository repository)
    {
        _repository = repository;
    }
    
    public Questionnaire GetQuestionnaire(int questionnaireId)
    {
        return _repository.ReadQuestionnaire(questionnaireId);
    }

    public bool EditQuestionnaire(Questionnaire questionnaire)
    {
        return _repository.UpdateQuestionnaire(questionnaire);
    }

    public Answer GetAnswer(int answerId)
    {
        return _repository.ReadAnswer(answerId);
    }
}