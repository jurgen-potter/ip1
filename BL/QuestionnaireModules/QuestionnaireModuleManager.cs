using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.DAL.QuestionnaireModules;

namespace CitizenPanel.BL.QuestionnaireModules;

public class QuestionnaireModuleManager(IQuestionnaireModuleRepository repository) : IQuestionnaireModuleManager
{
    public Questionnaire GetQuestionnaire(int questionnaireId)
    {
        return repository.ReadQuestionnaire(questionnaireId);
    }

    public bool EditQuestionnaire(Questionnaire questionnaire)
    {
        return repository.UpdateQuestionnaire(questionnaire);
    }

    public IEnumerable<Questionnaire> GetAllQuestionnaires()
    {
        return repository.ReadAllQuestionnaires();
    }

    public Answer GetAnswer(int answerId)
    {
        return repository.ReadAnswer(answerId);
    }
}