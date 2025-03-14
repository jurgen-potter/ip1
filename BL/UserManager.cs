using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;

namespace CitizenPanel.BL;

public class UserManager : IUserManager
{
    
    private IUserRepository _UserRepository;

    public UserManager(IUserRepository userRepository)
    {
        _UserRepository = userRepository;
    }
    
    
    public Panelmember AddPanelmember(string code, string email)
    {
        string hexAge = code.Substring(2, 4);
        int age = GetNumber(hexAge);
        string hexGender = code.Substring(12, 4);
        int genderNumber = GetNumber(hexGender);
        Gender gender = (Gender)genderNumber;
        string hexPanelId = code.Substring(21, 4);
        int panelId = GetNumber(hexPanelId);
        string postcode = $"{GetNumber(code[0].ToString())}{GetNumber(code[10].ToString())}" +
                          $"{GetNumber(code[19].ToString())}{GetNumber(code[27].ToString())}";

        Console.WriteLine($"{age},{gender},{panelId},{postcode}");
        
        Panelmember panelmember = new Panelmember()
        {
            Email = email,
            Age = age,
            Gender = gender,
            PanelId = panelId,
            Postcode = postcode
        };
        
        return panelmember;
    }

    
    public int GetNumber(string code)
    {
        int number = 0;
        for (int i = 0; i < code.Length; i++)
        {
            char c = code[i];
            if (c < 'G' && c >= 'A')
            {
                number += (10 + c - 'A') * (int)Math.Pow(16, (code.Length-1) - i);
            }
            else if (c >= '0' && c <= '9')
            {
                number += int.Parse(code[i].ToString()) * (int) Math.Pow(16, (code.Length-1) - i);
            }
        }
        return number;
    }
}