using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Panelmember : IdentityUser
{
    public string Email { get; set; }
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public int PanelId { get; set; }
    public string Postcode { get; set; }
}