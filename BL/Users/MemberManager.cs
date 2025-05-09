using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.QuestionnaireModules;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Panels;
using CitizenPanel.DAL.Users;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Users;

public class MemberManager(IMemberRepository memberRepository) : IMemberManager
{
    public ApplicationUser GetMemberById(string memberId)
    {
        return memberRepository.ReadUserById(memberId);
    }

    public ApplicationUser GetOrganizationByIdWithAnswers(string organizationId)
    {
        return memberRepository.ReadOrganizationByIdWithAnswers(organizationId);
    }

    public Task ChangeOrganizationAnswersAsync(string userId, int questionnaireId, List<Answer> answers)
    {
        return memberRepository.UpdateOrganizationAnswersAsync(userId, questionnaireId, answers);
    }
}