using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Panels;

public interface IMeetingManager
{
    Meeting GetMeetingByIdWithRecommendations(int id);
    Meeting AddMeeting(string title, DateOnly date,int panelId);
    
    void EditMeeting(Meeting meeting);
    
}