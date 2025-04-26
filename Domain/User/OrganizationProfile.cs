using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.User;

public class OrganizationProfile : ITenanted
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }
    public string TenantId { get; set; }
}