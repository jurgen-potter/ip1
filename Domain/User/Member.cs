using CitizenPanel.BL.Domain.PanelManagement;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

using Recruitment;

public class Member : IdentityUser
{
    public int MemberId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Town { get; set; }
    public Panel Panel { get; set; }
    public bool IsSelected { get; set; }

    public List<SubCriteria> SelectedCriteria { get; set; } = new List<SubCriteria>();
}