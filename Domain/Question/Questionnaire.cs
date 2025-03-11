namespace CitizenPanel.BL.Domain.Question;

public class Questionnaire
{
    public string Title { get; set; }
    public int Score { get; set; }
    public QuestionnaireType AmbitionQuestionnaire { get; set; }
    public QuestionnaireType PanelQuestionnaire { get; set; }
}