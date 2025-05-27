using CitizenPanel.UI.MVC.Models.Draws;

namespace CitizenPanel.UI.MVC.Models.DTO;

public class NewInvitationsDto
{
    public int PanelId { get; set; }
    public List<BucketViewModel> Buckets { get; set; }
}