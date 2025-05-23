using CitizenPanel.BL.Domain.Panels;
using CitizenPanel.DAL.Panels;

namespace CitizenPanel.BL.Panels;

public class PostManager(IPostRepository repository) : IPostManager
{
    public Post AddPost(string title, string description, DateTime date, string authorName, int panelId)
    {
        var post = new Post()
        {
            Title = title,
            Description = description,
            DatePosted = date,
            AuthorName = authorName,
            PanelId = panelId
        };
        repository.CreatePost(post);
        return post;
    }

    public Post GetPostById(int id)
    {
        return repository.ReadPostById(id);
    }

    public bool RemovePost(int id)
    {
        var post = GetPostById(id);
        return repository.DeletePost(post);
    }
}