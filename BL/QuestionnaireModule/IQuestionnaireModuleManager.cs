using CitizenPanel.BL.Domain.QuestionnaireModule;

namespace CitizenPanel.BL.QuestionnaireModule;

public interface IQuestionnaireModuleManager
{
    Questionnaire GetQuestionnaire(int questionnaireId);
    
    bool EditQuestionnaire(Questionnaire questionnaire);
    
    IEnumerable<Questionnaire> GetAllQuestionnaires();
}