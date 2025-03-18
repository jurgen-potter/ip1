using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers;

public class TryOutInvitationQRCodeController : Controller
{
    private readonly IDrawManager _drawManager;

    public TryOutInvitationQRCodeController(IDrawManager drawManager)
    {
        _drawManager = drawManager;
    }
    
    
    // GET
    public IActionResult Index()
    {
        IEnumerable<Invitation> invites = _drawManager.GetAllInvitations();
        return View(invites);
    }
}