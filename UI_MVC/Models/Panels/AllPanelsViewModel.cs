using System.ComponentModel.DataAnnotations;

namespace CitizenPanel.UI.MVC.Models.Panels;

public class AllPanelsViewModel
{
    public List<PanelSummaryViewModel> Panels { get; set; }

    [Display(Name = "Zoek op Panelnaam")] public string SearchPanelName { get; set; }

    [Display(Name = "Zoek op Organisatienaam")]
    public string SearchOrganisationName { get; set; }

    public AllPanelsViewModel()
    {
        Panels = new List<PanelSummaryViewModel>();
    }
}

public class PanelSummaryViewModel
{
    public string TenantId { get; set; }
    public int PanelId { get; set; }
    public string PanelName { get; set; }
    public string OrganisationName { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string Status { get; set; }
    public string FormattedStartDate => StartDate.ToString("dd MMM yyyy");
    public string FormattedEndDate => EndDate?.ToString("dd MMM yyyy") ?? "-";
}

