using CitizenPanel.BL.Domain.QuestionnaireModule;

namespace CitizenPanel.DAL.QuestionnaireModule;

public interface IQuestionnaireModuleRepository
{
    Questionnaire ReadQuestionnaire(int questionnaireId);

    bool UpdateQuestionnaire(Questionnaire questionnaire);
    
    IEnumerable<Questionnaire> ReadAllQuestionnaires();
    
    Answer ReadAnswer(int answerId);
}