namespace CitizenPanel.UI.MVC.Models.DTO;

public class PanelSelectViewModel
{
    public List<PanelSelectOptionViewModel> Panels { get; set; } = new List<PanelSelectOptionViewModel>();
}

public class PanelSelectOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}