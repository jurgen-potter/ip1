using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class ApplicationUser : IdentityUser
{
    public string TenantId { get; set; }
}