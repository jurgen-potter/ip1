using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draws;

public class DrawResult : ITenanted
{
    public int Id { get; set; }
    
    public ICollection<Invitation> SelectedInvitations { get; set; } = new List<Invitation>();
    public ICollection<Invitation> ReserveInvitations { get; set; } = new List<Invitation>();
    public ICollection<Invitation> NotSelectedInvitations { get; set; } = new List<Invitation>();
    public int TotalNeededPanelmembers { get; set; }
    public int ReservePanelmembers { get; set; }
    [Required]
    public string TenantId { get; set; }
}