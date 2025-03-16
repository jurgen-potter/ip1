using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Draw;

public class Invitation
{
    public string Code { get; set; }
    public string QRCodeString { get; set; }
    public int PanelId {get; set;}
    public Gender Gender { get; set; }
    public int Age { get; set; }
    public string Postcode { get; set; }
    
}