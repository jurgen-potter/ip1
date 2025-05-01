using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenPanel.UI.MVC.Views.TenantTesting;

[AllowAnonymous]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }
}