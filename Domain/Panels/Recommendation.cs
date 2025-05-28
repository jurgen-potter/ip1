using CitizenPanel.BL.Domain.Tenancy;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panels;

public class Recommendation : ITenanted
{
    public int Id { get; set; }
    public int MeetingId { get; set; }
    [Required] [MinLength(3)]
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsVotable { get; set; } = true;
    public int Votes { get; set; }
    public double NeededPercentage { get; set; }
    public bool IsAnonymous { get; set; }
    public ICollection<UserVote> UserVotes { get; set; }
    public bool Accepted { get; set; } = false;
    public bool IsDone { get; set; } = false;
    [Required]
    public string TenantId { get; set; }
}