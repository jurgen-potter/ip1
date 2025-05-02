using CitizenPanel.BL.Domain.QuestionnaireModule;

namespace CitizenPanel.DAL.QuestionnaireModule;

public interface IQuestionnaireModuleRepository
{
    public Questionnaire ReadQuestionnaire(int questionnaireId);

    public bool UpdateQuestionnaire(Questionnaire questionnaire);
    
    public Answer ReadAnswer(int answerId);
}