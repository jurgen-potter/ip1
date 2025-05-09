using CitizenPanel.BL.Domain.Tenancy;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panels;

public class Recommendation : ITenanted
{
    public int Id { get; set; }
    [Required] [MinLength(3)]
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsVotable { get; set; } = true;
    public int Votes { get; set; }
    public int NeededVotes { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public ICollection<UserVote> UserVotes { get; set; }
    [Required]
    public string TenantId { get; set; }
}