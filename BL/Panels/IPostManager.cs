using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.BL.Panels;

public interface IPostManager
{
    Post AddPost(string title,string description, DateTime date,String authorName,int panelId);
    Post GetPostById(int id);
    bool RemovePost(int id);
}