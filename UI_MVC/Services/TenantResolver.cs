using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Tenancy;

namespace CitizenPanel.UI.MVC.Services;

public class TenantResolver : ITenantResolver
{
    private readonly IDrawManager _drawManager;
    private readonly ITenantManager _tenantManager;

    public TenantResolver(IDrawManager drawManager, ITenantManager tenantManager)
    {
        _drawManager = drawManager;
        _tenantManager = tenantManager;
    }

    public Tenant ResolveTenantFromUser(ApplicationUser user)
    {
        if (user == null) return null;

        var tenantId =  user.UserType switch
        {
            UserType.Member when user.MemberProfile != null => user.MemberProfile.TenantId,
            UserType.Organization when user.OrganizationProfile != null => user.OrganizationProfile.TenantId,
            _ => null
        };
        return tenantId != null ? _tenantManager.GetTenantById(tenantId) : null;
    }

    public Tenant ResolveTenantFromQuery(HttpContext context)
    {
        string invitationCode = context.Request.Query["code"].ToString();
        if (string.IsNullOrEmpty(invitationCode))
        {
            return null;
        }
        
        var invitation = _drawManager.GetInvitationByCode(invitationCode);
        return invitation?.TenantId != null ? _tenantManager.GetTenantById(invitation.TenantId) : null;

    }
}