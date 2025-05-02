using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.Panel;

public class Meeting : ITenanted
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateOnly Date { get; set; }
    public ICollection<Recommendation> Recommendations { get; set; }
    public string TenantId { get; set; }
}