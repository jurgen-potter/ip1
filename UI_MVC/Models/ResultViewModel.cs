using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.UI.MVC.Models;

public class ResultViewModel
{
    public int ReservePotPanelmembers {get; set;}
    public int TotalNeededPanelmembers {get; set;}
    public List<BucketViewModel> Buckets { get; set; } = new List<BucketViewModel>();
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

public class BucketViewModel
{
    public List<string> CriteriaNames { get; set; } = new List<string>();
    public List<string> SubCriteriaNames { get; set; } = new List<string>();
    public int Count { get; set; }
}