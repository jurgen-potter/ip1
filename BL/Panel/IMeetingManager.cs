using CitizenPanel.BL.Domain.Panel;

namespace CitizenPanel.BL;

public interface IMeetingManager
{
    Meeting GetMeetingByIdWithRecommendations(int id);

}