using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Panels;

public interface IPanelManager
{
    Panel AddPanel(string name, string description, DateOnly endDate, ICollection<Criteria> criteria, int totAvPotMem);
    IEnumerable<Panel> GetAllPanels(); // wordt al gefilterd op tenant
    IEnumerable<Panel> GetAllPanelsWithoutTentant();
    IEnumerable<Panel> GetThreeActivePanels();
    Panel GetPanelById(int panelId);
    Panel GetPanelByIdWithMembers(int panelId);
    Panel GetPanelByIdWithInvitations(int panelId);
    Panel GetPanelByIdWithRecommendations(int panelId);
    Panel GetPanelByIdWithRecommendationsWithoutTenant(int panelId);
    Panel GetPanelByIdWithRecommendationsAndPosts(int panelId);
    Panel GetPanelByIdWithRecommendationsAndVotes(int panelId);
    void EditPanel(Panel panel);
    void RemovePanel(Panel panel);
    
    Recommendation GetRecommendationById(int recommendationDtoId);
    Recommendation GetRecommendationByIdWithVotes(int recommendationId);
    Recommendation GetRecommendationByIdWithVoters(int recommendationId);
    void EditRecommendation(Recommendation recommendation);

    IEnumerable<UserVote> GetUserVotesById(string userId);
    void AddUserVote(ApplicationUser member, Recommendation recommendation, bool recommended);
    void RemoveUserVote(ApplicationUser member, Recommendation recommendation);
    bool RemoveUserVotesByMember(ApplicationUser member);
    bool DoesUserVoteExist(ApplicationUser member, Recommendation recommendation);

    IEnumerable<Criteria> GetExtraCriteriaByPanelId(int panelId);
    IEnumerable<Criteria> GetCriteriaByPanelIdWithSubcriteria(int panelId);
    
    IEnumerable<ApplicationUser> GetMembersByPanelId(int panelId);

    IEnumerable<Meeting> GetMeetingsById(int panelId);
    
    IEnumerable<Invitation> GetReservesByPanelId(int panelId);
}