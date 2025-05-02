using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.DAL;

public interface IMeetingRepository
{
    Meeting ReadMeetingByIdWithRecommendations(int id);
    void CreateMeeting(Meeting meeting);
}