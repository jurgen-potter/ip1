namespace CitizenPanel.BL.Domain.PanelManagement;

public class Panel
{
    public string Name { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MemberCount { get; set; }
    public Ambition AmbitionPhase1 { get; set; }
    public Ambition AmbitionPhase2 { get; set; }
    public Ambition AmbitionPhase3 { get; set; }
}