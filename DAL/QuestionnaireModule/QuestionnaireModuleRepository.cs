using CitizenPanel.BL.Domain.QuestionnaireModule;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.QuestionnaireModule;

public class QuestionnaireModuleRepository : IQuestionnaireModuleRepository
{
    private readonly PanelDbContext _dbContext;

    public QuestionnaireModuleRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public Questionnaire ReadQuestionnaire(int questionnaireId)
    {
        return _dbContext.Questionnaires
            .Include(q => q.Questions.OrderBy(qp => qp.Position))
            .ThenInclude(a => a.Answers.OrderBy(ap => ap.Position))
            .SingleOrDefault(q => q.Id == questionnaireId);
    }

    public bool UpdateQuestionnaire(Questionnaire questionnaire)
    {
        _dbContext.Questionnaires.Update(questionnaire);
        return _dbContext.SaveChanges() > 0;
    }

    public Answer ReadAnswer(int answerId)
    {
        return _dbContext.Answers.Find(answerId);
    }
}