using CitizenPanel.BL.Domain.Content;
using CitizenPanel.DAL.Content;

namespace CitizenPanel.BL.Content;

public class ContentManager(IContentRepository repository) : IContentManager
{

    public InfoPageContent GetInfoPageContent()
    {
        return repository.ReadInfoPageContent();
    }

    public bool EditInfoPageContent(InfoPageContent infoPageContent)
    {
        return repository.UpdateInfoPageContent(infoPageContent);
    }

    public InfoSection AddInfoSection(string title, string text, string videoUrl, string fileUrl, int contentId)
    {
        var section = new InfoSection()
        {
            Title = title,
            Text = text,
            VideoUrl = videoUrl,
            FileUrl = fileUrl,
            InfoPageContentId = contentId
        };
        return repository.CreateInfoSection(section);
    }
    
    public void RemoveAllInfoSectionsByPageId(int pageId)
    {
        repository.DeleteAllInfoSectionsByPageId(pageId);
    }
}