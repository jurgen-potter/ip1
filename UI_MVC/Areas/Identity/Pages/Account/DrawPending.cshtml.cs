using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CitizenPanel.UI.MVC.Areas.Identity.Pages.Account;

public class DrawPendingModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        return RedirectToPage("/Index");
    }
}