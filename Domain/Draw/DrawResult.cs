using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult : ITenanted
{
    public int Id { get; set; }
    
    // public ICollection<ApplicationUser> SelectedMembers { get; set; } = new List<ApplicationUser>();
    // public ICollection<ApplicationUser> ReserveMembers { get; set; } = new List<ApplicationUser>();
    // public ICollection<ApplicationUser> NotSelectedMembers { get; set; } = new List<ApplicationUser>();
    public ICollection<MemberProfile> SelectedMembers { get; set; } = new List<MemberProfile>();
    public ICollection<MemberProfile> ReserveMembers { get; set; } = new List<MemberProfile>();
    public ICollection<MemberProfile> NotSelectedMembers { get; set; } = new List<MemberProfile>();
    
    public int TotalNeededPanelmembers { get; set; }
    public int ReservePanelmembers { get; set; }
    [Required]
    public string TenantId { get; set; }
}