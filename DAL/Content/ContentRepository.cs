using CitizenPanel.BL.Domain.Content;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Content;

public class ContentRepository(PanelDbContext dbContext) : IContentRepository
{
    public InfoPageContent ReadInfoPageContent()
    {
        return dbContext.InfoPageContents
            .Include(ip => ip.Sections)
            .FirstOrDefault();
    }

    public bool UpdateInfoPageContent(InfoPageContent infoPageContent)
    {
        dbContext.InfoPageContents.Update(infoPageContent);
        return dbContext.SaveChanges() > 0;
    }

    public InfoSection CreateInfoSection(InfoSection section)
    {
        dbContext.InfoSections.Add(section);
        dbContext.SaveChanges();
        return section;
    }
    
    public void DeleteAllInfoSectionsByPageId(int pageId)
    {
        var sections = dbContext.InfoSections.Where(s => s.InfoPageContentId == pageId);
        dbContext.InfoSections.RemoveRange(sections);
        dbContext.SaveChanges();
    }
}