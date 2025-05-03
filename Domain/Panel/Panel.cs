using System.ComponentModel.DataAnnotations;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL.Domain.Panel;

public class Panel : ITenanted
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public DateOnly StartDate { get; set; }
    
    public DateOnly EndDate { get; set; }
    
    public string CoverImagePath { get; set; }
    public ICollection<Meeting> Meetings { get; set; }
    
    public int MemberCount { get; set; }
    
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    
    public ICollection<MemberProfile> Members { get; set; } = new List<MemberProfile>();
    
    public OrganizationProfile Organization { get; set; }
    
    public ICollection<Criteria> Criteria { get; set; } = new List<Criteria>();
    public ICollection<RecruitmentBucket> RecruitmentBuckets { get; set; }
    public DrawStatus DrawStatus { get; set; }
    public DrawResult DrawResult { get; set; }
    [Required]
    public string TenantId { get; set; }
    
}