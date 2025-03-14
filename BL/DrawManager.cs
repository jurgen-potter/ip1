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
            string postCode = dummyMember.Postcode;
            
            string code = GenerateCode(age, genderNumber, panelId, postCode);

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


    public string GenerateCode(int age, int gender, int panelId, string postcode)
    {
        Random random = new Random();
        string code = string.Empty;
        string hexAge = age.ToString("X4");
        string hexGender = gender.ToString("X4");
        string hexPanelId = panelId.ToString("X4");
        string hexCode = $"{postcode[0]}-{hexAge}-000{postcode[1]}-{hexGender}-00{postcode[2]}-{hexPanelId}-0{postcode[3]}";
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