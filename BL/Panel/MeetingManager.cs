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

}