namespace CitizenPanel.BL.Domain.Panel;

public class UserVote
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int RecommendationId { get; set; }
    public Recommendation Recommendation { get; set; }
    public DateTime VotedAt { get; set; }
}