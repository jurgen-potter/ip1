using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.DAL.Questionnaires;

namespace CitizenPanel.BL.Questionnaires;

public class QuestionnaireManager(IQuestionnaireRepository repository) : IQuestionnaireManager
{
    public Questionnaire GetQuestionnaireById(int questionnaireId)
    {
        return repository.ReadQuestionnaireById(questionnaireId);
    }
    
    public IEnumerable<Questionnaire> GetAllQuestionnaires()
    {
        return repository.ReadAllQuestionnaires();
    }

    public bool EditQuestionnaire(Questionnaire questionnaire)
    {
        return repository.UpdateQuestionnaire(questionnaire);
    }

    public Answer GetAnswerById(int answerId)
    {
        return repository.ReadAnswerById(answerId);
    }
}