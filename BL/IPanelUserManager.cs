
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

using Domain.Recruitment;
using Microsoft.AspNetCore.Identity;

public interface IPanelUserManager
{
    public Panelmember AddPanelmember(string code, string email);
    public int GetNumber(string code);
    
    public Task<(IdentityResult result, IdentityUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria);
}