using CitizenPanel.BL.Domain.QuestionnaireModules;

namespace CitizenPanel.BL.QuestionnaireModules;

public interface IQuestionnaireModuleManager
{
    Questionnaire GetQuestionnaire(int questionnaireId);
    
    bool EditQuestionnaire(Questionnaire questionnaire);
    
    IEnumerable<Questionnaire> GetAllQuestionnaires();
    
    Answer GetAnswer(int answerId);
}