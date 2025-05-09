using CitizenPanel.BL.Domain.QuestionnaireModules;

namespace CitizenPanel.BL.QuestionnaireModules;

public interface IQuestionnaireModuleManager
{
    Questionnaire GetQuestionnaireById(int questionnaireId);
    
    bool ChangeQuestionnaire(Questionnaire questionnaire);
    
    IEnumerable<Questionnaire> GetAllQuestionnaires();
    
    Answer GetAnswerById(int answerId);
}