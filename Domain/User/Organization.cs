using CitizenPanel.BL.Domain.Tenancy;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL.Domain.User;

public class Organization : ApplicationUser, ITenanted
{
    public string TenantId { get; set; }
}