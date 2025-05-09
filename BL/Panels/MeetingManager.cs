using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class MeetingManager(IMeetingRepository repository) : IMeetingManager
{
    public Meeting AddMeeting(string title, DateOnly date, int panelId)
    {
        var meeting = new Meeting()
        {
            PanelId = panelId,
            Title = title,
            Date = date
        };
        repository.CreateMeeting(meeting);
        return meeting;
    }
    
    public Meeting GetMeetingByIdWithRecommendations(int id)
    {
       return repository.ReadMeetingByIdWithRecommendations(id);
    }

    public bool EditMeeting(Meeting meeting)
    {
        return repository.UpdateMeeting(meeting);
    }

}