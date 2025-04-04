using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.DAL.Registration;

public interface IRegistrationRepository
{
    
    
    public void UpdateDrawStatus(Panel panel);
}