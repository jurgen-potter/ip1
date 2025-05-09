using CitizenPanel.BL.Domain.Questionnaires;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.DAL.Users;

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