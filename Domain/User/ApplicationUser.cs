using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class ApplicationUser : IdentityUser
{
    public UserType UserType { get; set; }
    public MemberProfile MemberProfile { get; set; }
    public OrganizationProfile OrganizationProfile { get; set; }
}