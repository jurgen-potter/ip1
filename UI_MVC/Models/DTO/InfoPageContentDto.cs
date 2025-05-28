namespace CitizenPanel.UI.MVC.Models.DTO;

public class InfoPageContentDto
{
    public string MainTitle { get; set; }
    public List<InfoSectionDto> Sections { get; set; }
}

public class InfoSectionDto
{
    public string Title { get; set; }
    public string Text { get; set; }
    public string VideoUrl { get; set; }
    public string FileUrl { get; set; }
}