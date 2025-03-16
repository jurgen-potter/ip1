using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
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
        List<DummyMember> dummies = new List<DummyMember>();
        DummyMember dummy = new DummyMember()
        {
            PanelId = 1,
            Gender = Gender.Male,
            Age = 25,
            Postcode = "2140"
        };
        dummies.Add(dummy);
        
        List<Invitation> invites = _drawManager.AddInvitations(dummies);
        
        return View(invites);
    }
}