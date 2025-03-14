using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;

namespace CitizenPanel.BL;

public class DrawManager : IDrawManager
{
    private readonly IDrawRepository _drawRepository;

    public DrawManager(IDrawRepository drawRepository)
    {
        _drawRepository = drawRepository;
    }
    
    public List<Invitation> AddInvitations(List<DummyMember> members)
    {
        List<Invitation> invitations = new List<Invitation>();
        foreach (DummyMember dummyMember in members)
        {
            int age = dummyMember.Age;
            Gender gender = dummyMember.Gender;
            int genderNumber = (int)gender;
            int panelId = dummyMember.PanelId;
            string code = GenerateCode(age, genderNumber, panelId);

            Invitation invitation = new Invitation()
            {
                Code = code,
                Age = age,
                Gender = gender,
                PanelId = panelId
            };
            Invitation newInvitation = _drawRepository.AddInvitation(invitation);
            invitations.Add(invitation);
        }
        
        return invitations;
    }


    public string GenerateCode(int age, int gender, int panelId)
    {
        Random random = new Random();
        string code = string.Empty;
        string hexAge = age.ToString("X4");
        string hexGender = gender.ToString("X4");
        string hexPanelId = panelId.ToString("X4");
        string hexCode = $"0-{hexAge}-0000-{hexGender}-000-{hexPanelId}-00";
        foreach (char c in hexCode)
        {
            if (c == '0')
            {
                int replaceNumber = random.Next(0, 20);
                char letter = (char)('g' + replaceNumber);
                code += letter;
            }
            else
            {
                code += c;
            }
        }
        return code;
    }
}