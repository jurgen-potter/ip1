using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class PanelManager(IPanelRepository repository) : IPanelManager
{
    public Panel AddPanel(string name, string description, ICollection<Criteria> criteria, int totAvPotMem)
    {
        Panel newPanel = new Panel()
        {
            Name = name,
            Description = description,
            MemberCount = 0,
            Criteria = criteria,
            TotalAvailablePotentialPanelmembers = totAvPotMem
        };
        repository.CreatePanel(newPanel);
        return newPanel;
    }

    public Panel GetPanelById(int panelId)
    {
        return repository.ReadPanelById(panelId);
    }

    public IEnumerable<Panel> GetThreeActivePanels()
    {
        return repository.ReadThreeActivePanels();
    }


    public Panel GetPanelByIdWithMembers(int panelId)
    {
        return repository.ReadPanelByIdWithMembers(panelId);
    }

    public Panel GetPanelByIdWithInvitations(int panelId)
    {
        return repository.ReadPanelByIdWithInvitations(panelId);
    }
    
    public Panel GetPanelByIdWithRecommendations(int panelId)
    {
        return repository.ReadPanelByIdWithRecommendations(panelId);
    }
    
    public Panel GetPanelByIdWithRecommendationsWithoutTenant(int panelId)
    {
        return repository.ReadPanelByIdWithRecommendationsWithoutTenant(panelId);
    }


    public Panel GetPanelByIdWithRecommendationsAndPosts(int panelId)
    {
        return repository.ReadPanelByIdWithRecommendationsAndPosts(panelId);
    }

    
    public Panel GetPanelByIdWithRecommendationsAndVotes(int panelId)
    {
        return repository.ReadPanelByIdWithRecommendationsAndVotes(panelId);
    }
    
    public void EditPanel(Panel panel)
    {
        repository.UpdatePanel(panel);
    }

    public void RemovePanel(Panel panel)
    {
        repository.DeletePanel(panel);
    }
    
    public Recommendation GetRecommendationByIdWithVotes(int recommendationId)
    {
        return repository.ReadRecommendationByIdWithVotes(recommendationId);
    }
    
    public Recommendation GetRecommendationByIdWithVoters(int recommendationId)
    {
        return repository.ReadRecommendationByIdWithVoters(recommendationId);
    }

    public IEnumerable<UserVote> GetUserVotesById(string userId)
    {
        return repository.ReadUserVotesById(userId);
    }
    
    public void AddUserVote(ApplicationUser member, Recommendation recommendation, bool recommended)
    {
        if (recommendation == null)
        {
            throw new ArgumentException($"Aanbeveling met bestaat niet.");
        }

        if (DoesUserVoteExist(member, recommendation))
        {
            throw new InvalidOperationException("Gebruiker heeft al gestemd op deze aanbeveling.");
        }
        var userVote = new UserVote
        {
            Voter = member,
            Recommendation = recommendation,
            VotedAt = DateTime.UtcNow,
            Recommended = recommended,
            TenantId = member.MemberProfile.TenantId
        };
        
        repository.CreateUserVote(userVote);
        
        recommendation.Votes++;
        repository.UpdateRecommendation(recommendation);
        
    }

    public void RemoveUserVote(ApplicationUser member, Recommendation recommendation)
    {
        repository.DeleteUserVote(member, recommendation);
        if (recommendation.Votes > 0)
        {
            recommendation.Votes--;
        }
    }
    public bool RemoveUserVotesByMember(ApplicationUser member)
    {
        return repository.DeleteUserVotesByMember(member);
    }

    public bool DoesUserVoteExist(ApplicationUser member, Recommendation recommendation)
    {
        return repository.DoesUserVoteExist(member, recommendation);
    }
    
    public IEnumerable<Criteria> GetExtraCriteriaByPanelId(int panelId)
    {
        return repository.ReadExtraCriteriaByPanelId(panelId);
    }

    public IEnumerable<Criteria> GetCriteriaByPanelIdWithSubcriteria(int panelId)
    {
        return repository.ReadCriteriaByPanelIdWithSubcriteria(panelId);
    }

    public IEnumerable<Meeting> GetMeetingsById(int panelId)
    {
        return repository.ReadMeetingsById(panelId);
    }
    
    public void EditRecommendation(Recommendation recommendation)
    {
        repository.UpdateRecommendation(recommendation);
    }

    public IEnumerable<Panel> GetAllPanels()
    {
        return repository.ReadAllPanels();
    }

    public IEnumerable<Panel> GetAllPanelsWithoutTentant()
    {
        return repository.ReadAllPanelsWithoutTentant();
    }

    public IEnumerable<ApplicationUser> GetMembersByPanelId(int panelId)
    {
        return repository.ReadMembersByPanelId(panelId);
    }
}