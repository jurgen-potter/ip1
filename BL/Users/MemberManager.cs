using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.DAL.Users;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Users;

public class MemberManager(
    IDrawManager drawManager,
    IPanelManager panelManager,
    IMemberRepository memberRepository,
    UserManager<ApplicationUser> userManager) : IMemberManager
{
    public async Task<(IdentityResult result, ApplicationUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId)
    {
        List<SubCriteria> selectedCriteria = new List<SubCriteria>();
        
        if (newMemberSelectedCriteria != null && newMemberSelectedCriteria.Any())
        {
            foreach (var criteria in newMemberSelectedCriteria)
            {
                var crit = drawManager.GetSubCriteria(criteria);
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
                Panels = new List<Panel> { panelManager.GetPanelById(newMemberPanelId) }
            }
        };
        
        var result = await userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
    }

    public IEnumerable<ApplicationUser> GetAllMembers()
    {
        return memberRepository.ReadAllMembers();
    }

    public ApplicationUser GetMemberById(string memberId)
    {
        return memberRepository.ReadUserById(memberId);
    }

    public ApplicationUser GetOrganizationWithAnswers(string organizationId)
    {
        return memberRepository.ReadOrganizationWithAnswers(organizationId);
    }

    public void ChangeMember(ApplicationUser member)
    {
        memberRepository.UpdateMember(member);
    }

    public void RemoveMember(ApplicationUser member)
    {
        memberRepository.DeleteMember(member);
    }

    public Task ChangeOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        return memberRepository.UpdateOrganizationAnswersAsync(userId, questionnaireId, answers);
    }

    public IEnumerable<ApplicationUser> GetMembersOfPanelWithCriteria(int panelId)
    {
        return memberRepository.ReadMembersOfPanelWithCriteria(panelId);
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