using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace CitizenPanel.DAL.Panels;

public class MeetingRepository(PanelDbContext dbContext) : IMeetingRepository
{
    public void CreateMeeting(Meeting meeting)
    {
        dbContext.Meetings.Add(meeting);
        dbContext.SaveChanges();
    }
    
    public Meeting ReadMeetingByIdWithRecommendations(int id)
    {
        return dbContext.Meetings
            .Include(r => r.Recommendations)
            .FirstOrDefault(m => m.Id == id);
    }

    public bool UpdateMeeting(Meeting meeting)
    {
        dbContext.Meetings.Update(meeting);
        return dbContext.SaveChanges() > 0;
    }

}