using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public interface IRegistrationManager
{
    IEnumerable<RecruitmentBucket> GetInvitationBuckets(Panel panel);
    IEnumerable<RecruitmentBucket> GetAllBuckets(Panel panel);
    
    // New methods for draw phase management
    DrawStatus GetDrawStatus(Panel panel);
    void StartFinalDraw(Panel panel);
    bool HasSufficientRegistrations(Panel panel);
}