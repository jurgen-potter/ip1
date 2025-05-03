using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.DAL;

namespace CitizenPanel.BL;

public class MeetingManager : IMeetingManager
{
    private readonly IMeetingRepository _repository;

    public MeetingManager(IMeetingRepository repository)
    {
        _repository = repository;
    }
    public Meeting GetMeetingByIdWithRecommendations(int id)
    {
       return _repository.ReadMeetingByIdWithRecommendations(id);
    }

    public Meeting GetMeetingById(int id)
    {
        return _repository.ReadMeetingById(id);
    }

    public Meeting AddMeeting(string title, DateOnly date, int panelId)
    {
        var meeting = new Meeting()
        {
            PanelId = panelId,
            Title = title,
            Date = date
        };
       _repository.CreateMeeting(meeting);
       return meeting;
    }

    public void EditMeeting(Meeting meeting)
    {
        _repository.UpdateMeeting(meeting);
    }

}