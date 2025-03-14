using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.DAL;

public interface IPanelRepository
{
    public Panelmember CreatePanelmember(Panelmember panelmember);
}