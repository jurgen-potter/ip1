namespace CitizenPanel.UI.MVC.Models.QuestionnaireModules;

public class QuestionnaireSelectViewModel
{
    public ICollection<QuestionnaireViewModel> Questionnaires { get; set; } = new List<QuestionnaireViewModel>();
}

public class QuestionnaireViewModel
{
    public int Id { get; set; }
    public string Title { get; set; }
}