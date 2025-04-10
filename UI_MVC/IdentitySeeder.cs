using Microsoft.AspNetCore.Identity;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.UI.MVC;

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
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };
        await _userManager.CreateAsync(user1, "Admin123!");
        var token1 = await _userManager.GenerateEmailConfirmationTokenAsync(user1);
        await _userManager.ConfirmEmailAsync(user1, token1);
        await _userManager.AddToRoleAsync(user1, "Admin");
        
        // user 2
        var user2 = new Organization()
        {
            UserName = "antwerpen@example.com",
            Email = "antwerpen@example.com"
        };
        await _userManager.CreateAsync(user2, "Antwerpen1!");
        var token2 = await _userManager.GenerateEmailConfirmationTokenAsync(user2);
        await _userManager.ConfirmEmailAsync(user2, token2);
        await _userManager.AddToRoleAsync(user2, "Organization");
        
        // user 3
        var user3 = new Member()
        {
            UserName = "paul@example.com",
            Email = "paul@example.com"
        };
        await _userManager.CreateAsync(user3, "Paulpaul1!");
        var token3 = await _userManager.GenerateEmailConfirmationTokenAsync(user3);
        await _userManager.ConfirmEmailAsync(user3, token3);
    }
}