using CitizenPanel.BL.Domain.User;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;

using Domain.Recruitment;

public class PanelUserManager : IPanelUserManager
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IPanelManager _panelManager;

    public PanelUserManager(UserManager<IdentityUser> userManager, IPanelManager panelManager)
    {
        _userManager = userManager;
        _panelManager = panelManager;
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

    public async Task<(IdentityResult result, IdentityUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria)
    {
        List<SubCriteria> selectedCriteria = new List<SubCriteria>();
        foreach (var criteria in newMemberSelectedCriteria)
        {
            var crit = _panelManager.GetSubCriteria(criteria);
            selectedCriteria.Add(crit);
        }
        
        Member member = new Member()
        {
            FirstName = newMemberFirstName,
            LastName = newMemberLastName,
            Email = newMemberEmail,
            UserName = newMemberEmail,
            Gender = newMemberGender,
            BirthDate = newMemberBirthDate,
            Town = newMemberTown,
            SelectedCriteria = selectedCriteria
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
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