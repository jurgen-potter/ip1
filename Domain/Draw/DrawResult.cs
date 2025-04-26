using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult : ITenanted
{
    public int Id { get; set; }
    public ICollection<ApplicationUser> SelectedMembers { get; set; } = new List<ApplicationUser>();
    public ICollection<ApplicationUser> ReserveMembers { get; set; } = new List<ApplicationUser>();
    public ICollection<ApplicationUser> NotSelectedMembers { get; set; } = new List<ApplicationUser>();
    
    [Required]
    public string TenantId { get; set; }
}