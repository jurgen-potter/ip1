using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Registrations;

public interface IRegistrationManager
{
    IEnumerable<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<ApplicationUser> users);
    void StartFinalDraw(Panel panel);

}