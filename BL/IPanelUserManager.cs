using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

public interface IPanelUserManager
{
    public Panelmember AddPanelmember(string code, string email);
}