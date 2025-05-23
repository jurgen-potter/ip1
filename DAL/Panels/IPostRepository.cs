using CitizenPanel.BL.Domain.Panels;

namespace CitizenPanel.DAL.Panels;

public interface IPostRepository
{
    void CreatePost(Post post);
    Post ReadPostById(int id);
    bool DeletePost(Post post);
}