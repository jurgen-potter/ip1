using CitizenPanel.BL.Domain.Tenancy;
using CitizenPanel.BL.Domain.Users;
using CitizenPanel.UI.MVC.Areas.Identity.Managers;
using System.Security.Claims;
using CitizenPanel.BL.Draws;
using CitizenPanel.BL.Users;

namespace CitizenPanel.UI.MVC.Middleware;

public class TenantMiddleware(
    TenantContext tenantContext,
    ApplicationUserManager userManager,
    IDrawManager drawManager,
    IUserProfileManager userProfileManager) : IMiddleware
{
    private const string InvitationSessionKey = "CurrentInvitationContext";

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var user = userProfileManager.GetUserByIdWithProfile(userId);
                string tenantId = ResolveTenantFromUser(user);
                
                if (!string.IsNullOrEmpty(tenantId))
                {
                    tenantContext.Tenant = new Tenant { Id = tenantId };
                    await next(context);
                    return;
                }
            }
        }
        string tenantIdFromInvitation = ResolveTenantFromQuery(context);
        
        if (!string.IsNullOrEmpty(tenantIdFromInvitation))
        {
            tenantContext.Tenant = new Tenant { Id = tenantIdFromInvitation };
        }
        
        await next(context);
    }
    
    private string ResolveTenantFromUser(ApplicationUser user)
    {
        if (user == null) return null;
        
        if (user.UserType == UserType.Member && user.MemberProfile != null)
        {
            return user.MemberProfile.TenantId;
        }
        else if (user.UserType == UserType.Organization && user.OrganizationProfile != null)
        {
            return user.OrganizationProfile.TenantId;
        }
        
        return null;
    }
    
    private string ResolveTenantFromQuery(HttpContext context)
    {
        // Get tenant by getting the invitation from query
        string invitationCode = context.Request.Query["code"].ToString();
        if (!string.IsNullOrEmpty(invitationCode))
        {
            var invitation = drawManager.GetInvitationByCode(invitationCode);
            if (invitation != null && !string.IsNullOrEmpty(invitation.TenantId))
            {
                return invitation.TenantId;
            }
        }
        
        return null;
    }
}