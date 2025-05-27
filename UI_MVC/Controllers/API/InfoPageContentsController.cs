using CitizenPanel.BL.Content;
using CitizenPanel.UI.MVC.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CitizenPanel.UI.MVC.Controllers.API;

[Route("api/[controller]")]
[ApiController]
public class InfoPageContentsController(IContentManager contentManager) : ControllerBase
{
    [HttpGet]
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
                Text = section.Text
            };
            contentDto.Sections.Add(sectionDto);
        }

        return Ok(contentDto);
    }

    [HttpPost]
    public IActionResult SaveContents(InfoPageContentDto newContentDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        var content = contentManager.GetInfoPageContent();
        content.MainTitle = newContentDto.MainTitle;
        content.Sections.Clear();
        contentManager.RemoveAllInfoSectionsByPageId(content.Id);

        foreach (var sectionDto in newContentDto.Sections)
        {
            var section = contentManager.AddInfoSection(sectionDto.Title, sectionDto.Text, content.Id);
            content.Sections.Add(section);
        }

        contentManager.EditInfoPageContent(content);

        return Ok();
    }
}