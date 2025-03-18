
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

using Domain.Recruitment;
using Microsoft.AspNetCore.Identity;

public interface IPanelUserManager
{
    public Panelmember AddPanelmember(string code, string email);
    public int GetNumber(string code);
    
}