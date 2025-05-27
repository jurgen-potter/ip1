namespace CitizenPanel.UI.MVC.Models.Draws;

public class ResultViewModel
{
    public int TotalAvailablePotentialPanelmembers {get; set;}
    public int TotalNeededInvitations {get; set;}
    public int TotalNeededPanelmembers {get; set;}
    public List<BucketViewModel> Buckets { get; set; } = new List<BucketViewModel>();
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

public class BucketViewModel
{
    public List<string> CriteriaNames     { get; set; } = new();
    public List<string> SubCriteriaNames  { get; set; } = new();
    public int Count                      { get; set; }  
    public int RegisteredCount            { get; set; }  

    public bool IsSufficient => RegisteredCount >= Count;
}
