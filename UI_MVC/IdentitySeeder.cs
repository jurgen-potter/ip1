namespace CitizenPanel.UI.MVC;

using BL.Domain.User;
using Microsoft.AspNetCore.Identity;

public class IdentitySeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public IdentitySeeder(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    public async Task SeedAsync()
    {
        var adminRole = new IdentityRole("Admin");
        await _roleManager.CreateAsync(adminRole);
        var citizenRole = new IdentityRole("Citizen");
        await _roleManager.CreateAsync(citizenRole);
        var organizationRole = new IdentityRole("Organization");
        await _roleManager.CreateAsync(organizationRole);
        var potentialMemberRole = new IdentityRole("PotentialMember");
        await _roleManager.CreateAsync(potentialMemberRole);
        var panelMemberRole = new IdentityRole("PanelMember");
        await _roleManager.CreateAsync(panelMemberRole);
        
        // user 1
        var user1 = new Admin()
        {
            UserName = "admin@kdg.be",
            Email = "admin@kdg.be"
        };
        await _userManager.CreateAsync(user1, "Admin123!");
        await _userManager.AddToRoleAsync(user1, "Admin");
        
        // user 2
        var user2 = new Citizen()
        {
            UserName = "chris@kdg.be",
            Email = "chris@kdg.be"
        };
        await _userManager.CreateAsync(user2, "Chrischris1!");
        await _userManager.AddToRoleAsync(user2, "Citizen");
        
        // user 3
        var user3 = new Organization()
        {
            UserName = "antwerpen@antwerpen.be",
            Email = "antwerpen@antwerpen.be"
        };
        await _userManager.CreateAsync(user3, "Antwerpen1!");
        await _userManager.AddToRoleAsync(user3, "Organization");
        
        // user 4
        var user4 = new Member()
        {
            UserName = "paul@kdg.be",
            Email = "paul@kdg.be"
        };
        await _userManager.CreateAsync(user4, "Paulpaul1!");
        await _userManager.AddToRoleAsync(user4, "PanelMember");
    }
}