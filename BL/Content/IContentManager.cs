using CitizenPanel.BL.Domain.Content;

namespace CitizenPanel.BL.Content;

public interface IContentManager
{
    InfoPageContent GetInfoPageContent();
    bool EditInfoPageContent(InfoPageContent infoPageContent);
    
    InfoSection AddInfoSection(string title, string text, int contentId);
    void RemoveAllInfoSectionsByPageId(int pageId);
}