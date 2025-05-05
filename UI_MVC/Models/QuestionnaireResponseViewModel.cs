using CitizenPanel.BL.Domain.QuestionnaireModules;

namespace CitizenPanel.UI.MVC.Models;

public class QuestionnaireResponseViewModel
{ 
    public int QuestionnaireId { get; set; }
    public Questionnaire Questionnaire { get; set; }
    public bool IsCritical { get; set; }
    public Dictionary<int, int> Answers { get; set; } = new();
    public List<int> SelectedAnswerIds { get; set; } = new();
}