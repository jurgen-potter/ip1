using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.UI.MVC.Models.DTO;

public class MemberDto
{
    public string Code { get; set; }
    public bool IsConfirmed { get; set; }
    public Invitation Invitation { get; set; }
    public List<Criteria> CriteriaList { get; set; }

}