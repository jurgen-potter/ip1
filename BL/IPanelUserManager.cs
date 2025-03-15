
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

using Domain.Recruitment;
using Microsoft.AspNetCore.Identity;

public interface IPanelUserManager
{
    public Panelmember AddPanelmember(string code, string email);
    
    public Task<Member> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<SubCriteria> newMemberSelectedCriteria);
}