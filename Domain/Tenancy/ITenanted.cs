namespace CitizenPanel.BL.Domain.Tenancy;

public interface ITenanted
{
    public string TenantId { get; set; }
}