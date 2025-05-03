using CitizenPanel.BL.Domain.Tenancy;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panel;

public class Recommendation : ITenanted
{
    public int Id { get; set; }
    [Required] [MinLength(3)]
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsVotable { get; set; }
    public int Votes { get; set; }
    public ICollection<UserVote> UserVotes { get; set; }
    [Required]
    public string TenantId { get; set; }

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