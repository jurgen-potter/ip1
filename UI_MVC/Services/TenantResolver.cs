using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Draws;

namespace CitizenPanel.UI.MVC.Services;

public class TenantResolver : ITenantResolver
{
    private readonly IDrawManager _drawManager;

    public TenantResolver(IDrawManager drawManager)
    {
        _drawManager = drawManager;
    }

    public string ResolveTenantFromUser(ApplicationUser user)
    {
        if (user == null) return null;

        return user.UserType switch
        {
            UserType.Member when user.MemberProfile != null => user.MemberProfile.TenantId,
            UserType.Organization when user.OrganizationProfile != null => user.OrganizationProfile.TenantId,
            _ => null
        };
    }

    public string ResolveTenantFromQuery(HttpContext context)
    {
        string invitationCode = context.Request.Query["code"].ToString();
        if (!string.IsNullOrEmpty(invitationCode))
        {
            var invitation = _drawManager.GetInvitationByCode(invitationCode);
            if (invitation != null && !string.IsNullOrEmpty(invitation.TenantId))
            {
                return invitation.TenantId;
            }
        }

        return null;
    }
}