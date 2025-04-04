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
        var organizationRole = new IdentityRole("Organization");
        await _roleManager.CreateAsync(organizationRole);
        
        // user 1
        var user1 = new IdentityUser()
        {
            UserName = "admin@kdg.be",
            Email = "admin@kdg.be"
        };
        await _userManager.CreateAsync(user1, "Admin123!");
        await _userManager.AddToRoleAsync(user1, "Admin");
        
        
        // user 2
        var user2 = new Organization()
        {
            UserName = "antwerpen@antwerpen.be",
            Email = "antwerpen@antwerpen.be"
        };
        await _userManager.CreateAsync(user2, "Antwerpen1!");
        await _userManager.AddToRoleAsync(user2, "Organization");
        
        // user 3
        var user3 = new Member()
        {
            UserName = "paul@kdg.be",
            Email = "paul@kdg.be"
        };
        await _userManager.CreateAsync(user3, "Paulpaul1!");
    }
}