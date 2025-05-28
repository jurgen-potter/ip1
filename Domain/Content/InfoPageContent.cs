namespace CitizenPanel.BL.Domain.Content;

public class InfoPageContent
{
    public int Id { get; set; }
    public string MainTitle { get; set; }
    public ICollection<InfoSection> Sections { get; set; } = new List<InfoSection>();
}

public class InfoSection
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public string VideoUrl { get; set; }
    public string FileUrl { get; set; }

    public int InfoPageContentId { get; set; }
    public InfoPageContent InfoPageContent { get; set; }
}