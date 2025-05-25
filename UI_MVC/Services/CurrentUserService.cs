using CitizenPanel.DAL.ServiceInterfaces;

namespace CitizenPanel.UI.MVC.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public bool IsAdmin => _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") == true;
}