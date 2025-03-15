using CitizenPanel.BL.Domain.PanelManagement;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Member : IdentityUser
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    // public string Email { get; set; }  Standaard met IdentityUser
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string Town { get; set; }
    public Panel Panel { get; set; }
    public bool IsSelected { get; set; }

}