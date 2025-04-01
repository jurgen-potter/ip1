using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.DAL.Registration;

public interface IRegistrationRepository
{
    public ICollection<RecruitmentBucket> ReadTargetBucketsByPanel(Panel panel);
    
    public void updateDrawStatus(Panel panel);
}