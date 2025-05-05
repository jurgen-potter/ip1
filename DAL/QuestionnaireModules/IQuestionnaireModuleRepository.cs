using CitizenPanel.BL.Domain.QuestionnaireModules;

namespace CitizenPanel.DAL.QuestionnaireModules;

public interface IQuestionnaireModuleRepository
{
    Questionnaire ReadQuestionnaire(int questionnaireId);

    bool UpdateQuestionnaire(Questionnaire questionnaire);
    
    IEnumerable<Questionnaire> ReadAllQuestionnaires();
    
    Answer ReadAnswer(int answerId);
}