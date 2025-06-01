using System.ComponentModel.DataAnnotations.Schema;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.BL.Domain.Draws;

public class Invitation
{
    public int Id { get; set; }
    public string Code { get; set; }
    public byte[] QRCode { get; set; }
    public int PanelId {get; set;}
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string Town { get; set; }
    public bool IsRegistered { get; set; }
    public bool IsDrawn { get; set; }
    public List<int> SelectedCriteria { get; set; }
    public string Email { get; set; }
    public string UserId { get; set; }
    public string TenantId { get; set; }
    public int Batch { get; set; }
    
    [NotMapped]
    public string QRCodeBase64 => QRCode != null ? Convert.ToBase64String(QRCode) : null;
}