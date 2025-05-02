using CitizenPanel.BL.Domain.QuestionnaireModule;

namespace CitizenPanel.BL.QuestionnaireModule;

public interface IQuestionnaireModuleManager
{
    public Questionnaire GetQuestionnaire(int questionnaireId);
    
    public bool EditQuestionnaire(Questionnaire questionnaire);
    
    public Answer GetAnswer(int answerId);
}