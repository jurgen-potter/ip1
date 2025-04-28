using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.UI.MVC.Models;

public class ResultsViewModel
{
    public int MaleCount { get; set; }
    public int FemaleCount { get; set; }
    public int Age18_25Count { get; set; }
    public int Age26_40Count { get; set; }
    public int Age41_60Count { get; set; }
    public int Age60PlusCount { get; set; }
    public int ReservePotPanelmembers {get; set;}
    public double TotalNeededPanelmembers {get; set;}

    public IList<CriteriaResult> CriteriaResults { get; set; }

}