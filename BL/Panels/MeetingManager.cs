using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class MeetingManager(IMeetingRepository repository) : IMeetingManager
{
    public Meeting AddMeeting(string title, DateOnly date, int panelId)
    {
        if (panelId <= 0)
        {
            throw new ArgumentException($"Invalid panelId: {panelId}. PanelId must be greater than 0.");
        }
        
        if (string.IsNullOrEmpty(title))
        {
            throw new ArgumentException("Meeting title cannot be null or empty.");
        }
        
        var meeting = new Meeting()
        {
            PanelId = panelId,
            Title = title,
            Date = date
        };
        repository.CreateMeeting(meeting);
        return meeting;
    }
    public Meeting GetMeetingById(int id)
    {
        return repository.ReadMeetingById(id);
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