using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.QuestionnaireModules;

public class QuestionnaireModuleRepository(PanelDbContext dbContext) : IQuestionnaireModuleRepository
{
    public Questionnaire ReadQuestionnaire(int questionnaireId)
    {
        return dbContext.Questionnaires
            .Include(q => q.Questions.OrderBy(qp => qp.Position))
            .ThenInclude(a => a.Answers.OrderBy(ap => ap.Position))
            .SingleOrDefault(q => q.Id == questionnaireId);
    }

    public bool UpdateQuestionnaire(Questionnaire questionnaire)
    {
        dbContext.Questionnaires.Update(questionnaire);
        return dbContext.SaveChanges() > 0;
    }

    public IEnumerable<Questionnaire> ReadAllQuestionnaires()
    {
        return dbContext.Questionnaires;
    }

    public Answer ReadAnswer(int answerId)
    {
        return dbContext.Answers.Find(answerId);
    }
}