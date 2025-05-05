using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panels;

public class UserVote : ITenanted
{
    public int Id { get; set; }
    public ApplicationUser Voter { get; set; }
    public bool Recommended { get; set; }
    public Recommendation Recommendation { get; set; }
    public DateTime VotedAt { get; set; }
    [Required]
    public string TenantId { get; set; }
}