using CitizenPanel.BL.Domain.Questionnaires;

namespace CitizenPanel.DAL.Questionnaires;

public interface IQuestionnaireRepository
{
    Questionnaire ReadQuestionnaireById(int questionnaireId);
    IEnumerable<Questionnaire> ReadAllQuestionnaires();
    bool UpdateQuestionnaire(Questionnaire questionnaire);
    
    Answer ReadAnswerById(int answerId);
}