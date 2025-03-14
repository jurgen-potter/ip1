using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Member : IdentityUser
{
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    // public string Email { get; set; }  Standaard met IdentityUser
    public Gender Gender { get; set; }
    public Age Age { get; set; }
    public string Town { get; set; }
}