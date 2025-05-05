using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.DAL.Panels;

public interface IMeetingRepository
{
    Meeting ReadMeetingByIdWithRecommendations(int id);
    Meeting ReadMeetingById(int id);
    void CreateMeeting(Meeting meeting);
    void UpdateMeeting(Meeting meeting);
}