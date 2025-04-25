using CitizenPanel.BL.Domain.Tenancy;
using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Draw;

public class Invitation : ITenanted
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string QRCodeString { get; set; }
    public int PanelId {get; set;}
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string Postcode { get; set; }
    public bool IsRegistered { get; set; }
    public bool IsDrawn { get; set; }
    public List<int> SelectedCriteria { get; set; }
    public string Email { get; set; }
    [Required]
    public string TenantId { get; set; }
    
}