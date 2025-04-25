using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panel;

public class UserVote : ITenanted
{
    public int Id { get; set; }
    public Member Voter { get; set; }
    public int RecommendationId { get; set; }
    public Recommendation Recommendation { get; set; }
    public DateTime VotedAt { get; set; }
    [Required]
    public string TenantId { get; set; }
}