using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL;

public class MeetingRepository : IMeetingRepository
{
    private readonly PanelDbContext _dbContext;

    public MeetingRepository(PanelDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Meeting ReadMeetingByIdWithRecommendations(int id)
    {
        return _dbContext.Meetings.Include(r => r.Recommendations)
            .FirstOrDefault(m => m.Id == id);
    }
}