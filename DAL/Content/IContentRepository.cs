using CitizenPanel.BL.Domain.Content;

namespace CitizenPanel.DAL.Content;

public interface IContentRepository
{
    InfoPageContent ReadInfoPageContent();
    bool UpdateInfoPageContent(InfoPageContent infoPageContent);
    
    InfoSection CreateInfoSection(InfoSection section);
    void DeleteAllInfoSectionsByPageId(int pageId);
}