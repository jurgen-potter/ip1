using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.Panel;

public class Meeting : ITenanted
{
    public int id { get; set; }
    //public string Title
    public DateOnly Date { get; set; }
    public ICollection<Recommendation> Recommendations { get; set; }
    public string TenantId { get; set; }
}