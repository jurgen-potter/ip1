using System.Security.Claims;
using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models.Panels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.Panels;

[Authorize(Roles = "Organization")]
public class PostController : Controller
{
    private readonly IPostManager _postManager;

    public PostController(IPostManager postManager)
    {
        _postManager = postManager;
    }

    [HttpGet]
    public IActionResult Create(int panelId)
    {
        if (panelId <= 0)
        {
            return BadRequest("Een geldig panel ID is vereist.");
        }

        var model = new CreatePostViewModel
        {
            PanelId = panelId
        };
        
        return View(model);
    }

    [HttpPost]
    public IActionResult Create(CreatePostViewModel model)
    {
        if (model.PanelId <= 0)
        {
            ModelState.AddModelError("PanelId", "Een geldig panel ID is vereist.");
        }
        
        if (ModelState.IsValid)
        {
            try
            {
                string authorName = User.Identity?.Name ?? User.FindFirstValue(ClaimTypes.Email) ?? "Onbekende Auteur";

                _postManager.AddPost(
                    model.Title,
                    model.Content,
                    DateTime.UtcNow,
                    authorName,
                    model.PanelId
                );

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }

                TempData["SuccessMessage"] = "Post succesvol aangemaakt!";
                return RedirectToAction("Details", "Panel", new { panelId = model.PanelId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Er is een fout opgetreden: {ex.Message}");
            }
        }
        
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return Json(new { success = false, errors = ModelState.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            )});
        }

        return RedirectToAction("Details", "Panel", new { panelId = model.PanelId });
    }
}