using CitizenPanel.BL.Domain.Draw;
using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models;

public class RecruitmentCriteriaViewModel
{
    public int PanelId { get; set; }

    public int TotalAvailablePotentialPanelmembers { get; set; }

    // Alle criteria (bijv. “Gender”, “Age”, “Transport”, …)
    public List<CriteriaViewModel> Criteria { get; set; } = new();
}

public class CriteriaViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } // bv. “Gender”, “Age”, “Transport”
    public List<SubCriteriaViewModel> SubCriteria { get; set; } = new();
}

public class SubCriteriaViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } // bv. “Male”, “Female” of “18-25”, “Bus”, etc.
    public double Percentage { get; set; }
}

