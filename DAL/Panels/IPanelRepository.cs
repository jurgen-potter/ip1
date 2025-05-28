using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.DAL.Panels;

public interface IPanelRepository
{
    void CreatePanel(Panel panel);
    IEnumerable<Panel> ReadAllPanels(); // wordt al gefilterd op tenant
    IEnumerable<Panel> ReadAllPanelsWithoutTentant();
    IEnumerable<Panel> ReadThreeActivePanels();
    Panel ReadPanelById(int panelId);
    Panel ReadPanelByIdWithMembers(int panelId);
    Panel ReadPanelByIdWithInvitations(int panelId);
    Panel ReadPanelByIdWithRecommendations(int panelId);
    Panel ReadPanelByIdWithRecommendationsWithoutTenant(int panelId);
    Panel ReadPanelByIdWithRecommendationsAndPosts(int panelId);
    Panel ReadPanelByIdWithRecommendationsAndVotes(int panelId);
    void UpdatePanel(Panel panel);
    void DeletePanel(Panel panel);
    
    Recommendation ReadRecommendationById(int recommendationDtoId);
    Recommendation ReadRecommendationByIdWithVotes(int recommendationId);
    Recommendation ReadRecommendationByIdWithVoters(int recommendationId);
    void UpdateRecommendation(Recommendation recommendation);
    
    IEnumerable<UserVote> ReadUserVotesById(string userId);
    void CreateUserVote(UserVote userVote);
    void DeleteUserVote(ApplicationUser member, Recommendation recommendation);
    bool DeleteUserVotesByMember(ApplicationUser member);
    bool DoesUserVoteExist(ApplicationUser member, Recommendation recommendation);

    IEnumerable<Criteria> ReadExtraCriteriaByPanelId(int panelId);
    IEnumerable<Criteria> ReadCriteriaByPanelIdWithSubcriteria(int panelId);
    
    IEnumerable<ApplicationUser> ReadMembersByPanelId(int panelId);

    IEnumerable<Meeting> ReadMeetingsById(int panelId);
}