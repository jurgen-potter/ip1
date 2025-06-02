using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Panels;

public interface IMeetingManager
{
    Meeting AddMeeting(string title, DateOnly date,int panelId);
    Meeting GetMeetingById(int id);
    Meeting GetMeetingByIdWithRecommendations(int id);
    bool EditMeeting(Meeting meeting);
}