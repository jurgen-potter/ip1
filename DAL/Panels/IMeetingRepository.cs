using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.DAL.Panels;

public interface IMeetingRepository
{
    void CreateMeeting(Meeting meeting);
    Meeting ReadMeetingByIdWithRecommendations(int id);
    bool UpdateMeeting(Meeting meeting);
}