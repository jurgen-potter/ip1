using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public class MemberManager : IMemberManager
{
    private readonly IDrawManager _drawManager;
    private readonly IPanelManager _panelManager;
    private readonly IMemberRepository _memberRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public MemberManager(IDrawManager drawManager, IPanelManager panelManager, IMemberRepository memberRepository, UserManager<ApplicationUser> userManager)
    {
        _drawManager = drawManager;
        _panelManager = panelManager;
        _memberRepository = memberRepository;
        _userManager = userManager;
    }
    
    public async Task<(IdentityResult result, ApplicationUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId)
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
        
        ApplicationUser member = new ApplicationUser()
        {
            Email = newMemberEmail,
            UserName = newMemberEmail,
            MemberProfile = new MemberProfile()
            {
                FirstName = newMemberFirstName,
                LastName = newMemberLastName,
                Gender = newMemberGender,
                BirthDate = newMemberBirthDate,
                Town = newMemberTown,
                SelectedCriteria = selectedCriteria,
                Panels = new List<Panel> { _panelManager.GetPanelById(newMemberPanelId) }
            }
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
    }

    public IEnumerable<ApplicationUser> GetAllMembers()
    {
        return _memberRepository.ReadAllMembers();
    }

    public ApplicationUser GetMemberById(string memberId)
    {
        return _memberRepository.ReadUserById(memberId);
    }

    public void ChangeMember(ApplicationUser member)
    {
        _memberRepository.UpdateMember(member);
    }

    public void RemoveMember(ApplicationUser member)
    {
        _memberRepository.DeleteMember(member);
    }

    public IEnumerable<ApplicationUser> GetMembersOfPanelWithCriteria(int panelId)
    {
        return _memberRepository.ReadMembersOfPanelWithCriteria(panelId);
    }

    /*public IEnumerable<ApplicationUser> GetMembersByPanelId(int panelId)
    {
        return _memberRepository.ReadMembersByPanelId(panelId);
    }

    public IEnumerable<ApplicationUser> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _memberRepository.ReadMembersByPanelIdGenderAndAgeRange(panelId, gender, minAge, maxAge);
    }

    public int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _memberRepository.ReadMemberCountByPanelIdGenderAndAgeRange(panelId, gender, minAge, maxAge);
    }*/
    
}