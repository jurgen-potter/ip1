namespace CitizenPanel.DAL.ServiceInterfaces;

public interface ICurrentUserService
{
    bool IsAdmin { get; }
}