namespace CitizenPanel.UI.MVC.Models.DTO;

public class MeetRecDto
{
    public int MeetingId { get; set; }
    public string MeetingTitle { get; set; }
    public int AmountVotable { get; set; }
    public List<int> RecIds { get; set; }
    public List<string> RecTitles { get; set; }
    public List<string> RecDescriptions { get; set; }
    public List<bool> RecAnon { get; set; }
    public List<bool> RecVotable { get; set; }
    public List<int> RecVotes { get; set; }
    public List<int> RecVotesFor { get; set; }
    public List<int> RecVotesAgainst { get; set; }
    public List<double> RecNeededPercentages { get; set; }
}