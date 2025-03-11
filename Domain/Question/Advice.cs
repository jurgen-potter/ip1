namespace CitizenPanel.BL.Domain.Question;

public class Advice
{
    public string ScoreRange { get; set; }
    public string Description { get; set; }
    public ICollection<string> Links { get; set; }
}