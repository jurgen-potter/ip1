using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.Panels;

public class Meeting : ITenanted
{
    public int Id { get; set; }
    public int PanelId { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public ICollection<Recommendation> Recommendations { get; set; }
    public string TenantId { get; set; }
}