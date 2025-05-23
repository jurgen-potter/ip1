using CitizenPanel.BL.Domain.Tenancy;

namespace CitizenPanel.BL.Domain.Panels;

public class Post : ITenanted
{
    public int Id { get; set; } 
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DatePosted { get; set; }
    public string AuthorName { get; set; }
    public int PanelId { get; set; }
    public string TenantId { get; set; }

}