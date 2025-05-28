namespace CitizenPanel.UI.MVC.Models.DTO;

public class MeetingDto
{
    public int MeetingId { get; set; }
    public string MeetingTitle { get; set; }
    public int Participants { get; set; }
    public int AmountVotable { get; set; }
    public List<RecDto> Recs { get; set; }
}

public class RecDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Anonymous { get; set; }
    public bool Votable { get; set; }
    public int Votes { get; set; }
    public int VotesFor { get; set; }
    public int VotesAgainst { get; set; }
    public double NeededPercentages { get; set; }
    public bool IsDone { get; set; }
}