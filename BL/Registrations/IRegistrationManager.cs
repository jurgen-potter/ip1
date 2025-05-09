using CitizenPanel.BL.Domain.Draws;
using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Registrations;

public interface IRegistrationManager
{
    List<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<Invitation> registeredInvitations);
    void StartFinalDraw(Panel panel);
}