using CitizenPanel.BL.Domain.Questionnaires;

namespace CitizenPanel.BL.Questionnaires;

public interface IQuestionnaireManager
{
    Questionnaire GetQuestionnaireById(int questionnaireId);
    IEnumerable<Questionnaire> GetAllQuestionnaires();
    bool EditQuestionnaire(Questionnaire questionnaire);
    
    Answer GetAnswerById(int answerId);
}