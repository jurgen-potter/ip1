namespace CitizenPanel.UI.MVC.Models.Panels;

public class PanelSelectViewModel
{
    public List<PanelSelectOptionViewModel> Panels { get; set; } = new List<PanelSelectOptionViewModel>();
}

public class PanelSelectOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CoverImagePath { get; set; }
    public string TenantId { get; set; }

}