
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

public interface IUserManager
{
    public Panelmember AddPanelmember(string code, string email);
}