using CitizenPanel.BL.Content;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class InfoPageContentsController(IContentManager contentManager) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetContents()
    {
        var content = contentManager.GetInfoPageContent();
        var contentDto = new InfoPageContentDto()
        {
            MainTitle = content.MainTitle,
            Sections = new List<InfoSectionDto>()
        };

        foreach (var section in content.Sections)
        {
            var sectionDto = new InfoSectionDto()
            {
                Title = section.Title,
                Text = section.Text,
                VideoUrl = section.VideoUrl,
                FileUrl = section.FileUrl
            };
            contentDto.Sections.Add(sectionDto);
        }

        return Ok(contentDto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult SaveContents(InfoPageContentDto newContentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var content = contentManager.GetInfoPageContent();
        var oldVideoUrls = content.Sections.Select(s => s.VideoUrl).Where(u => !string.IsNullOrEmpty(u)).ToList();
        var oldFileUrls = content.Sections.Select(s => s.FileUrl).Where(u => !string.IsNullOrEmpty(u)).ToList();
        
        content.MainTitle = newContentDto.MainTitle;
        content.Sections.Clear();
        contentManager.RemoveAllInfoSectionsByPageId(content.Id);

        foreach (var sectionDto in newContentDto.Sections)
        {
            contentManager.AddInfoSection(sectionDto.Title, sectionDto.Text, sectionDto.VideoUrl, sectionDto.FileUrl, content.Id);
        }

        contentManager.EditInfoPageContent(content);
        
        var newVideoUrls = newContentDto.Sections.Select(s => s.VideoUrl).Where(u => !string.IsNullOrEmpty(u)).ToList();
        var newFileUrls = newContentDto.Sections.Select(s => s.FileUrl).Where(u => !string.IsNullOrEmpty(u)).ToList();

        var removedVideoUrls = oldVideoUrls.Except(newVideoUrls);
        var removedFileUrls = oldFileUrls.Except(newFileUrls);

        // Delete files from wwwroot/uploads for removed URLs
        foreach (var url in removedVideoUrls.Concat(removedFileUrls))
        {
            DeleteFileIfExists(url);
        }

        return Ok();
    }
    
    private void DeleteFileIfExists(string url)
    {
        // url format assumed: "/uploads/filename.ext"
        if (string.IsNullOrEmpty(url)) return;

        var fileName = Path.GetFileName(url);
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        var filePath = Path.Combine(uploadsFolder, fileName);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }
    }
}