using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Panels;

public interface IPanelRepository
{
    void CreatePanel(Panel panel);
    IEnumerable<Panel> ReadAllPanels(); // wordt al gefilterd op tenant
    Panel ReadPanelById(int panelId);
    Panel ReadPanelByIdWithMembers(int panelId);
    Panel ReadPanelByIdWithInvitations(int panelId);
    Panel ReadPanelByIdWithRecommendations(int panelId);
    Panel ReadPanelByIdWithRecommendationsAndVotes(int panelId);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
    
    Recommendation ReadRecommendationByIdWithVotes(int recommendationId);
    Recommendation ReadRecommendationByIdWithVoters(int recommendationId);
    void UpdateRecommendation(Recommendation recommendation);
    
    void CreateUserVote(UserVote userVote);
    void DeleteUserVote(ApplicationUser member, Recommendation recommendation);
    bool DoesUserVoteExist(ApplicationUser member, Recommendation recommendation);

    IEnumerable<Criteria> ReadExtraCriteriaByPanelId(int panelId);
    IEnumerable<Criteria> ReadCriteriaByPanelIdWithSubcriteria(int panelId);
}