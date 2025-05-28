using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Data;

namespace CitizenPanel.DAL.Panels;

public class PostRepository(PanelDbContext dbContext) : IPostRepository
{
    public void CreatePost(Post post)
    {
        dbContext.Posts.Add(post);
        dbContext.SaveChanges();
    }

    public Post ReadPostById(int id)
    {
       return dbContext.Posts.Find(id);
    }

    public bool DeletePost(Post post)
    {
        dbContext.Posts.Remove(post);
        dbContext.SaveChanges();
        return true;
    }
}