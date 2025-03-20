using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.DAL;

//NugetPackage nog toevoegen aan DAL
public interface IDrawRepository
{
    public Invitation CreateInvitation(Invitation invitation);
    public Invitation ReadInvitationWithCode(string code);
    public IEnumerable<Invitation> ReadAllInvitations();
    public Invitation UpdateInvitation(Invitation invitation);
    
    public ExtraCriteria ReadExtraCriteria(int criteriaId);
    
    public IEnumerable<ExtraCriteria> ReadAllExtraCriteria();
    
    public SubCriteria ReadSubCriteria(int subCriteriaId);
    IEnumerable<ExtraCriteria> ReadExtraCriteriaByPanel(int panelId);
}