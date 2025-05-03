using System.Runtime.InteropServices.JavaScript;
using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IMeetingManager
{
    Meeting GetMeetingByIdWithRecommendations(int id);
    Meeting GetMeetingById(int id);
    Meeting AddMeeting(string title, DateOnly date,int panelId);
    
    void EditMeeting(Meeting meeting);
    
    void AddRecommendationToMeeting(int meetingId, Recommendation recommendation);
}