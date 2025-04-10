using CitizenPanel.BL.Domain.QuestionnaireModule;

namespace CitizenPanel.DAL.QuestionnaireModule;

public interface IQuestionnaireModuleRepository
{
    public Questionnaire ReadQuestionnaire(int questionnaireId);

    public bool UpdateRepository(Questionnaire questionnaire);
}