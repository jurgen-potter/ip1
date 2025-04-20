using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panel;

public class Recommendation
{
    public int Id { get; set; }
    [Required] [MinLength(3)]
    public string Title { get; set; }
    public string Description { get; set; }
    public int Votes { get; set; }

    public ICollection<UserVote> UserVotes { get; set; }

    public Recommendation(string title, string description, int votes)
    {
        Title = title;
        Description = description;
        Votes = votes;
    }
    public Recommendation()
    {
        UserVotes = new List<UserVote>();
    }
}