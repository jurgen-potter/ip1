using CitizenPanel.BL.Domain.Users;
using CitizenPanel.BL.Users;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[ApiController]
[Route("/api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminsController(IUserProfileManager userProfileManager, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var currentUser = userManager.GetUserAsync(User).Result;
        if (currentUser is { IsSuper: false })
        {
            return Forbid("Alleen root admins kunnen admins verwijderen.");
        }
        
        var admins = userProfileManager.GetAllAdmins()
            .Where(u => u.Id != currentUser?.Id)
            .OrderByDescending(u => u.IsSuper)
            .ToList();
        
        if (admins.Count == 0) {
            return NoContent();
        }
        
        List<AdminDto> adminDtos = [];
        foreach (var admin in admins)
        {
            var adminDto = new AdminDto()
            {
                Id = admin.Id,
                Email = admin.Email,
                IsSuper = admin.IsSuper
            };
            adminDtos.Add(adminDto);
        }
        
        return Ok(adminDtos);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = userProfileManager.GetUserById(id);

        if (user == null)
            return NotFound();

        if (user.UserType != UserType.Admin)
            return BadRequest("User is not an admin.");
        
        var currentUser = await userManager.GetUserAsync(User);
        if (currentUser is { IsSuper: false })
        {
            return Forbid("Alleen root admins kunnen admins verwijderen.");
        }

        await userManager.DeleteAsync(user);
    
        return NoContent();
    }

}