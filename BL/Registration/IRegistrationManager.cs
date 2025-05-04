using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Registration;

public interface IRegistrationManager
{
    IEnumerable<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<MemberProfile> profiles);
    void StartFinalDraw(Panel panel);

}