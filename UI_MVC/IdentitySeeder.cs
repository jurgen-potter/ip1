using Microsoft.AspNetCore.Identity;
using CitizenPanel.BL.Domain.Users;

namespace CitizenPanel.UI.MVC;

public class IdentitySeeder(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
{
    public async Task SeedAsync()
    {
        var adminRole = new IdentityRole("Admin");
        await roleManager.CreateAsync(adminRole);
        var organizationRole = new IdentityRole("Organization");
        await roleManager.CreateAsync(organizationRole);
        var memberRole = new IdentityRole("Member");
        await roleManager.CreateAsync(memberRole);
        
        // user 1
        var user1 = new ApplicationUser()
        {
            UserName = "admin@example.com",
            Email = "admin@example.com"
        };
        await userManager.CreateAsync(user1, "Test1!");
        var token1 = await userManager.GenerateEmailConfirmationTokenAsync(user1);
        await userManager.ConfirmEmailAsync(user1, token1);
        await userManager.AddToRoleAsync(user1, "Admin");
        
        // user 2
        var user2 = new ApplicationUser()
        {
            UserName = "antwerpen@example.com",
            Email = "antwerpen@example.com"
        };
        await userManager.CreateAsync(user2, "Test1!");
        
        user2.OrganizationProfile = new OrganizationProfile()
        {
            TenantId = "antwerpen"
        };
        user2.UserType = UserType.Organization;
        await userManager.UpdateAsync(user2);
        
        var token2 = await userManager.GenerateEmailConfirmationTokenAsync(user2);
        await userManager.ConfirmEmailAsync(user2, token2);
        await userManager.AddToRoleAsync(user2, "Organization");
        
        // user 3
        var user3 = new ApplicationUser()
        {
            UserName = "paul@example.com",
            Email = "paul@example.com"
        };
        await userManager.CreateAsync(user3, "Test1!");
        
        user3.MemberProfile = new MemberProfile()
        {
            Gender = Gender.Male,
            TenantId = "antwerpen"
        };
        user3.UserType = UserType.Member;
        await userManager.UpdateAsync(user2);
        
        var token3 = await userManager.GenerateEmailConfirmationTokenAsync(user3);
        await userManager.ConfirmEmailAsync(user3, token3);
        await userManager.AddToRoleAsync(user3, "Member");
        
        // user 4
        var user4 = new ApplicationUser()
        {
            UserName = "brussel@example.com",
            Email = "brussel@example.com"
        };
        await userManager.CreateAsync(user4, "Test1!");
        
        user4.OrganizationProfile = new OrganizationProfile()
        {
            TenantId = "brussel"
        };
        user4.UserType = UserType.Organization;
        await userManager.UpdateAsync(user4);
        
        var token4 = await userManager.GenerateEmailConfirmationTokenAsync(user4);
        await userManager.ConfirmEmailAsync(user4, token4);
        await userManager.AddToRoleAsync(user4, "Organization");
    }
}