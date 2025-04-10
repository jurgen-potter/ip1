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
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .SingleOrDefault(q => q.Id == questionnaireId);
    }

    public bool UpdateRepository(Questionnaire questionnaire)
    {
        _dbContext.Questionnaires.Update(questionnaire);
        return _dbContext.SaveChanges() > 0;
    }
}