using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public class MemberManager : IMemberManager
{
    private readonly IDrawManager _drawManager;
    private readonly IPanelManager _panelManager;
    private readonly UserManager<IdentityUser> _userManager;

    public MemberManager(IDrawManager drawManager, IPanelManager panelManager, UserManager<IdentityUser> userManager)
    {
        _drawManager = drawManager;
        _panelManager = panelManager;
        _userManager = userManager;
    }
    
    public async Task<(IdentityResult result, IdentityUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId)
    {
        List<SubCriteria> selectedCriteria = new List<SubCriteria>();
        
        if (newMemberSelectedCriteria != null && newMemberSelectedCriteria.Any())
        {
            foreach (var criteria in newMemberSelectedCriteria)
            {
                var crit = _drawManager.GetSubCriteria(criteria);
                if (crit != null)
                {
                    selectedCriteria.Add(crit);
                }
            }
        }
        
        Member member = new Member()
        {
            FirstName = newMemberFirstName,
            LastName = newMemberLastName,
            Email = newMemberEmail,
            UserName = newMemberEmail,
            Gender = newMemberGender,
            BirthDate = newMemberBirthDate,
            Town = newMemberTown,
            SelectedCriteria = selectedCriteria,
            Panel = _panelManager.GetPanel(newMemberPanelId)
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
    }

    public IEnumerable<Member> GetAllMembers()
    {
        throw new NotImplementedException();
    }

    public Member GetMemberById(int memberId)
    {
        throw new NotImplementedException();
    }

    public void ChangeMember(Member member)
    {
        throw new NotImplementedException();
    }

    public void RemoveMember(Member member)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Member> GetMembersByPanelId(int panelId)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Member> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        throw new NotImplementedException();
    }

    public int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        throw new NotImplementedException();
    }

    public void MarkMembersAsSelected(IEnumerable<Member> members)
    {
        throw new NotImplementedException();
    }
}