using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class ApplicationUser : IdentityUser
{
    public UserType UserType { get; set; }
    public MemberProfile MemberProfile { get; set; }
    public OrganizationProfile OrganizationProfile { get; set; }
}

public class MemberProfile : ITenanted
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string TenantId { get; set; }
    public string FirstName { get; set; } 
    public string LastName { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public DateOnly BirthDate { get; set; }
    public string Town { get; set; }
    public ICollection<Panel.Panel> Panels { get; set; } = new List<Panel.Panel>();
    public List<SubCriteria> SelectedCriteria { get; set; } = new List<SubCriteria>();
}

public class OrganizationProfile : ITenanted
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string TenantId { get; set; }
}