using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using QRCoder;

namespace CitizenPanel.BL;

public class DrawManager : IDrawManager
{
    private readonly IDrawRepository _drawRepository;

    public DrawManager(IDrawRepository drawRepository)
    {
        _drawRepository = drawRepository;
    }
    
    public IEnumerable<Invitation> AddInvitations(List<DummyMember> members)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        List<Invitation> invitations = new List<Invitation>();
        foreach (DummyMember dummyMember in members)
        {
            int age = dummyMember.Age;
            Gender gender = dummyMember.Gender;
            int genderNumber = (int)gender;
            int panelId = dummyMember.PanelId;
            string postCode = dummyMember.Postcode;
            string code = GenerateCode();

            string qrCodePlace = "https://whimp-24.ew.r.appspot.com/MemberRegister/RegisterMember?code=" + code; 
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrCodePlace, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeAsPngByteArr = qrCode.GetGraphic(20);
            string qrCodeString = Convert.ToBase64String(qrCodeAsPngByteArr);

            
            Invitation invitation = new Invitation()
            {
                Code = code,
                Age = age,
                Gender = gender,
                PanelId = panelId,
                QRCodeString = qrCodeString,
            };
            Invitation newInvitation = _drawRepository.CreateInvitation(invitation);
            invitations.Add(newInvitation);
        }
        
        return invitations;
    }

    public Invitation GetInvitationWithCode(string code)
    {
        return _drawRepository.ReadInvitationWithCode(code);
    }

    public IEnumerable<Invitation> GetAllInvitations()
    {
        return _drawRepository.ReadAllInvitations();
    }

    public Invitation ChangeInvitation(Invitation invitation)
    {
        return _drawRepository.UpdateInvitation(invitation);
    }
    
    public string GenerateCode()
    {
        Random random = new Random();
        string code = String.Empty;
        string codeTemplate = $"0000-0000-0000-0000-0000";
        foreach (char c in codeTemplate)
        {
            char newLetDig = c;
            if (newLetDig == '0')
            {
                int replaceNumber = random.Next(0, 62);
                if (replaceNumber >= 36)
                {
                    newLetDig = (char)('a' + replaceNumber - 36);
                }
                else if (replaceNumber >= 10)
                {
                    newLetDig = (char)('A' + replaceNumber - 10);
                }
                else
                {
                    newLetDig = (char)('0' + replaceNumber);
                }
            }
            code += newLetDig;

        }
        return code;
    }
    
    
    public ExtraCriteria GetExtraCriteria(int criteriaId)
    {
        return _drawRepository.ReadExtraCriteria(criteriaId);
    }
    
    public IEnumerable<ExtraCriteria> GetAllExtraCriteria()
    {
        return _drawRepository.ReadAllExtraCriteria();
    }
    
    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return _drawRepository.ReadSubCriteria(subCriteriaId);
    }
    
    public IEnumerable<ExtraCriteria> GetExtraCriteriaByPanel(int panelId)
    {
        return _drawRepository.ReadExtraCriteriaByPanel(panelId);
    }

    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, double malePercentage, double femalePercentage, double age18_25Percentage, double age26_40Percentage, double age41_60Percentage, double age60PlusPercentage, List<ExtraCriteria> extraCriteria)
    {
        var reservePerc = 0.08;
        var totalAvailablePotPanelmembers = 0.424 * Math.Sqrt(totalAvailablePotentialPanelmembers);

        var result = new RecruitmentResult
        {
            MaleCount = (int)(totalAvailablePotPanelmembers * (malePercentage / 100)),
            FemaleCount = (int)(totalAvailablePotPanelmembers * (femalePercentage / 100)),
            Age18_25Count = (int)(totalAvailablePotPanelmembers * (age18_25Percentage / 100)),
            Age26_40Count = (int)(totalAvailablePotPanelmembers * (age26_40Percentage / 100)),
            Age41_60Count = (int)(totalAvailablePotPanelmembers * (age41_60Percentage / 100)),
            Age60PlusCount = (int)(totalAvailablePotPanelmembers * (age60PlusPercentage / 100)),
            ReservePotPanelmembers = (int)(totalAvailablePotPanelmembers / reservePerc),
            TotalNeededPanelmembers = (int)(totalAvailablePotPanelmembers),
            ExtraCriteriaResults = new List<CriteriaResult>()
        };

        // Extra criteria (wordt overgenomen zoals eerder)
        foreach (var extra in extraCriteria)
        {
            var criteriaResult = new CriteriaResult
            {
                Name = extra.Name,
                SubResults = extra.SubCriteria.Select(sub => new SubCriteriaResult
                {
                    Name = sub.Name,
                    Count = (int)(totalAvailablePotPanelmembers * (sub.Percentage / 100))
                }).ToList()
            };
            result.ExtraCriteriaResults.Add(criteriaResult);
        }

        return result;
    }
}