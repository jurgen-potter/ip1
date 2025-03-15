using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.BL;

using Domain.Recruitment;
using Microsoft.AspNetCore.Identity;

public class PanelUserManager : IPanelUserManager
{
    private readonly UserManager<IdentityUser> _userManager;

    public PanelUserManager(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
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

    public async Task<Member> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<SubCriteria> newMemberSelectedCriteria)
    {
        Member member = new Member()
        {
            FirstName = newMemberFirstName,
            LastName = newMemberLastName,
            Email = newMemberEmail,
            UserName = newMemberEmail,
            Gender = newMemberGender,
            BirthDate = newMemberBirthDate,
            Town = newMemberTown,
            //SelectedCriteria = newMemberSelectedCriteria
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? member : null;
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