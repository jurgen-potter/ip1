using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Organization : IdentityUser
{
    public PanelRoles Roles { get; set; }
}