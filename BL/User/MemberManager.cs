using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public class MemberManager : IMemberManager
{
    private readonly IDrawManager _drawManager;
    private readonly IPanelManager _panelManager;
    private readonly IMemberRepository _memberRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public MemberManager(IDrawManager drawManager, IPanelManager panelManager, IMemberRepository memberRepository, UserManager<IdentityUser> userManager)
    {
        _drawManager = drawManager;
        _panelManager = panelManager;
        _memberRepository = memberRepository;
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
            Panel = _panelManager.GetPanelById(newMemberPanelId)
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
    }

    public IEnumerable<Member> GetAllMembers()
    {
        return _memberRepository.ReadAllMembers();
    }

    public Member GetMemberById(string memberId)
    {
        return _memberRepository.ReadMemberById(memberId);
    }

    public void ChangeMember(Member member)
    {
        _memberRepository.UpdateMember(member);
    }

    public void RemoveMember(Member member)
    {
        _memberRepository.DeleteMember(member);
    }

    public IEnumerable<Member> GetMembersByPanelId(int panelId)
    {
        return _memberRepository.ReadMembersByPanelId(panelId);
    }

    public IEnumerable<Member> GetMembersByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _memberRepository.ReadMembersByPanelIdGenderAndAgeRange(panelId, gender, minAge, maxAge);
    }

    public int GetMemberCountByPanelIdGenderAndAgeRange(int panelId, Gender gender, int minAge, int maxAge)
    {
        return _memberRepository.ReadMemberCountByPanelIdGenderAndAgeRange(panelId, gender, minAge, maxAge);
    }
    
}