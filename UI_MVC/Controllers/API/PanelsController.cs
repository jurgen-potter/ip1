using CitizenPanel.BL.Panels;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;
[ApiController]
[Route("{tenantId}/api/[controller]")]
public class PanelsController(IPanelManager panelManager) : ControllerBase
{
    [HttpPut("edit")]
    public IActionResult EditPanel([FromBody] PanelDto model)
    {
        var panel = panelManager.GetPanelByIdWithRecommendationsWithoutTenant(model.Id);
        panel.ShowRejectedRecommendations = model.ShowRejected;
        panelManager.EditPanel(panel);
        
        List<RecDto> recs = new List<RecDto>();
        foreach (var meeting in panel.Meetings)
        {
            foreach (var recommendation in meeting.Recommendations.Where(r => !r.IsVotable && !r.Accepted))
            {
                RecDto recommendationDto = new RecDto()
                {
                    Id = recommendation.Id,
                    Title = recommendation.Title,
                    Description = recommendation.Description
                };
                recs.Add(recommendationDto);
            }
        }
        
        return Ok(recs);
    }
    
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetPanel(int id)
    {
        var panel = panelManager.GetPanelById(id);
        return panel == null ? NotFound() : Ok(panel);
    }

    [Authorize(Roles = "Organization")]
    [HttpPost("{id}")]
    public IActionResult UpdatePanel(EditPanelDto panelDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var panel = panelManager.GetPanelById(panelDto.Id);
        if (panel == null) return NotFound();

        panel.Name = panelDto.Name;
        panel.Description = panelDto.Description;
        panel.EndDate = panelDto.EndDate;

        panelManager.EditPanel(panel);
        return Ok();
    }

    [Authorize(Roles = "Organization")]
    [HttpPost("{panelId}/UploadBanner")]
    public async Task<IActionResult> UploadBannerImage(int panelId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest();

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bannerUploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var panel = panelManager.GetPanelById(panelId);
        panel.BannerImagePath = $"/bannerUploads/{uniqueFileName}";
        panelManager.EditPanel(panel);

        return Ok(new { path = panel.BannerImagePath });
    }
    
    [Authorize(Roles = "Organization")]
    [HttpPost("{panelId}/UploadCover")]
    public async Task<IActionResult> UploadCoverImage(int panelId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest();

        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "coverUploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var panel = panelManager.GetPanelById(panelId);
        panel.CoverImagePath = $"/coverUploads/{uniqueFileName}";
        panelManager.EditPanel(panel);

        return Ok(new { path = panel.CoverImagePath });
    }
}