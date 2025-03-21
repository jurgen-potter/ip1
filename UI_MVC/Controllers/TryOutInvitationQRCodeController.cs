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
        List<DummyMember> members = new List<DummyMember>();

        DummyMember dummy1 = new DummyMember()
        {
            Age = 30,
            Gender = Gender.Female,
            PanelId = 1,
            Postcode = "2140"
        };
            
        DummyMember dummy2 = new DummyMember()
        {
            Age = 20,
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2100"
        };
        
        DummyMember dummy3 = new DummyMember()
        {
            Age = 40,
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2110"
        };
        
        DummyMember dummy4 = new DummyMember()
        {
            Age = 40,
            Gender = Gender.Female,
            PanelId = 2,
            Postcode = "2105"
        };
        
        DummyMember dummy5 = new DummyMember()
        {
            Age = 25,
            Gender = Gender.Male,
            PanelId = 1,
            Postcode = "2140"
        };

        members.Add(dummy1);
        members.Add(dummy2);
        members.Add(dummy3);
        members.Add(dummy4);
        members.Add(dummy5);
        
        IEnumerable<Invitation> invitesDummy = _drawManager.AddInvitations(members);
        
        IEnumerable<Invitation> invites = _drawManager.GetAllInvitations();
        return View(invites);
    }
}