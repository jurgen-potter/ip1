using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Member : ApplicationUser, ITenanted
{
    public int MemberId { get; set; }
    public string TenantId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Town { get; set; }
    public Panel.Panel Panel { get; set; }
    public List<SubCriteria> SelectedCriteria { get; set; } = new List<SubCriteria>();
}