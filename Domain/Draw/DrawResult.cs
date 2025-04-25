using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Draw;

public class DrawResult : ITenanted
{
    public int Id { get; set; }
    public ICollection<Member> SelectedMembers { get; set; } = new List<Member>();
    public ICollection<Member> ReserveMembers { get; set; } = new List<Member>();
    public ICollection<Member> NotSelectedMembers { get; set; } = new List<Member>();
    
    [Required]
    public string TenantId { get; set; }
}