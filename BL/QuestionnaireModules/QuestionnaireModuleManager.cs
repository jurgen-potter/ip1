using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.DAL.QuestionnaireModules;

namespace CitizenPanel.BL.QuestionnaireModules;

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

    public IEnumerable<Questionnaire> GetAllQuestionnaires()
    {
        return _repository.ReadAllQuestionnaires();
    }

    public Answer GetAnswer(int answerId)
    {
        return _repository.ReadAnswer(answerId);
    }
}