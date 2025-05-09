using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Questionnaires;

public class QuestionnaireRepository(PanelDbContext dbContext) : IQuestionnaireRepository
{
    public Questionnaire ReadQuestionnaireById(int questionnaireId)
    {
        return dbContext.Questionnaires
            .Include(q => q.Questions.OrderBy(qp => qp.Position))
            .ThenInclude(a => a.Answers.OrderBy(ap => ap.Position))
            .SingleOrDefault(q => q.Id == questionnaireId);
    }
    
    public IEnumerable<Questionnaire> ReadAllQuestionnaires()
    {
        return dbContext.Questionnaires.ToList();
    }

    public bool UpdateQuestionnaire(Questionnaire questionnaire)
    {
        dbContext.Questionnaires.Update(questionnaire);
        return dbContext.SaveChanges() > 0;
    }

    public Answer ReadAnswerById(int answerId)
    {
        return dbContext.Answers.Find(answerId);
    }
}