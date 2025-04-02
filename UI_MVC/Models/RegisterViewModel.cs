using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.UI.MVC.Models;

public class RegisterViewModel
{
    public List<ExtraCriteria> CriteriaList { get; set; }
    
    public List<int> SelectedCriteria { get; set; }
    
    public Invitation Invitation { get; set; }
    
    public bool IsConfirmed { get; set; }

}