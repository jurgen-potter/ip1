using Microsoft.AspNetCore.Identity;
using CitizenPanel.BL.Domain.User;

namespace CitizenPanel.UI.MVC;

public class IdentitySeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    
    public IdentitySeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
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
        var memberRole = new IdentityRole("Member");
        await _roleManager.CreateAsync(memberRole);
        
        // user 1
        var user1 = new ApplicationUser()
        {
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };
        await _userManager.CreateAsync(user1, "Admin123!");
        var token1 = await _userManager.GenerateEmailConfirmationTokenAsync(user1);
        await _userManager.ConfirmEmailAsync(user1, token1);
        await _userManager.AddToRoleAsync(user1, "Admin");
        
        // user 2
        var user2 = new ApplicationUser()
        {
            UserName = "antwerpen@example.com",
            Email = "antwerpen@example.com"
        };
        await _userManager.CreateAsync(user2, "Antwerpen1!");
        
        user2.OrganizationProfile = new OrganizationProfile()
        {
            TenantId = "antwerpen"
        };
        user2.UserType = UserType.Organization;
        await _userManager.UpdateAsync(user2);
        
        var token2 = await _userManager.GenerateEmailConfirmationTokenAsync(user2);
        await _userManager.ConfirmEmailAsync(user2, token2);
        await _userManager.AddToRoleAsync(user2, "Organization");
        
        // user 3
        var user3 = new ApplicationUser()
        {
            UserName = "paul@example.com",
            Email = "paul@example.com"
        };
        await _userManager.CreateAsync(user3, "Paulpaul1!");
        
        user3.MemberProfile = new MemberProfile()
        {
            TenantId = "antwerpen"
        };
        user3.UserType = UserType.Member;
        await _userManager.UpdateAsync(user2);
        
        var token3 = await _userManager.GenerateEmailConfirmationTokenAsync(user3);
        await _userManager.ConfirmEmailAsync(user3, token3);
        await _userManager.AddToRoleAsync(user3, "Member");
    }
}