using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.Panel;

public class UserVote
{
    public int Id { get; set; }
    public Member Voter { get; set; }
    public int RecommendationId { get; set; }
    public Recommendation Recommendation { get; set; }
    public DateTime VotedAt { get; set; }
}