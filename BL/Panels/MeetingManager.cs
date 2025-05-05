using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class MeetingManager(IMeetingRepository repository) : IMeetingManager
{
    public Meeting GetMeetingByIdWithRecommendations(int id)
    {
       return repository.ReadMeetingByIdWithRecommendations(id);
    }

    public Meeting GetMeetingById(int id)
    {
        return repository.ReadMeetingById(id);
    }

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

    public void EditMeeting(Meeting meeting)
    {
        repository.UpdateMeeting(meeting);
    }

}