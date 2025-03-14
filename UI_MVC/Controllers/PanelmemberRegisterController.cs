using CitizenPanel.BL;
using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Mvc;

namespace UI_MVC.Controllers;

public class PanelmemberRegisterController : Controller
{
    private readonly IDrawManager _drawManager;

    public PanelmemberRegisterController(IDrawManager drawManager)
    {
        _drawManager = drawManager;
    }
    
    // GET
    public IActionResult Index()
    {
        List<DummyMember> members = new List<DummyMember>();
        DummyMember dummy1 = new DummyMember()
        {
            Age = 10,
            Gender = Gender.Male,
            PanelId = 1
        };
        DummyMember dummy2 = new DummyMember()
        {
            Age = 34,
            Gender = Gender.Male,
            PanelId = 16
        };
        DummyMember dummy3 = new DummyMember()
        {
            Age = 52,
            Gender = Gender.Female,
            PanelId = 17
        };
        DummyMember dummy4 = new DummyMember()
        {
            Age = 48,
            Gender = Gender.Female,
            PanelId = 2
        };
        DummyMember dummy5 = new DummyMember()
        {
            Age = 80,
            Gender = Gender.Male,
            PanelId = 1
        };
        DummyMember dummy6 = new DummyMember()
        {
            Age = 19,
            Gender = Gender.Female,
            PanelId = 1
        };
        members.Add(dummy1);
        members.Add(dummy2);
        members.Add(dummy3);
        members.Add(dummy4);
        members.Add(dummy5);
        members.Add(dummy6);

        List<Invitation> invitations = _drawManager.AddInvitations(members);
        
        return View(invitations);
    }
    
    
}