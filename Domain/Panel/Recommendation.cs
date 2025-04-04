using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.BL.Domain.Panel;

public class Recommendation
{
    public int Id { get; set; }
    [Required] [MinLength(3)]
    public string Title { get; set; }
    public string Description { get; set; }
    public int Votes { get; set; }


    public Recommendation(int id,string title, string description, int votes)
    {
        Id = id;
        Title = title;
        Description = description;
        Votes = votes;
    }
}