using CitizenPanel.BL.Domain.Draw;

namespace CitizenPanel.DAL;

//NugetPackage nog toevoegen aan DAL
public interface IDrawRepository
{
    public Invitation CreateInvitation(Invitation invitation);
    public Invitation ReadInvitationWithCode(string code);
    public IEnumerable<Invitation> ReadAllInvitations();
    public Invitation UpdateInvitation(Invitation invitation);
    
    public SubCriteria CreateSubCriteria(SubCriteria subCriteria);
    public Criteria CreateCriteria(Criteria criteria);
    public Criteria ReadCriteria(int criteriaId);
    
    public IEnumerable<Criteria> ReadAllCriteria();
    
    public SubCriteria ReadSubCriteria(int subCriteriaId);
    IEnumerable<Criteria> ReadCriteriaByPanel(int panelId);
    public bool DeleteInvitation(int invitationId);
}