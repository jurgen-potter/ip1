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
        var totalAvailablePotPanelmembers = Math.Round(0.50 * Math.Sqrt(totalAvailablePotentialPanelmembers));


        var maleCount = (int)(totalAvailablePotPanelmembers * (malePercentage / 100));
        var femaleCount = (int)(totalAvailablePotPanelmembers * (femalePercentage / 100));
        var age18_25Count = (int)(totalAvailablePotPanelmembers * (age18_25Percentage / 100));
        var age26_40Count = (int)(totalAvailablePotPanelmembers * (age26_40Percentage / 100));
        var age41_60Count = (int)(totalAvailablePotPanelmembers * (age41_60Percentage / 100));
        var age60PlusCount = (int)(totalAvailablePotPanelmembers * (age60PlusPercentage / 100));
        var reservePotPanelmembers = (int)(totalAvailablePotPanelmembers / reservePerc);
        var totalNeededPanelmembers = (int)(totalAvailablePotPanelmembers);

        var genderCount = maleCount + femaleCount;
        while (genderCount < totalNeededPanelmembers)
        {
            var neededGender = totalNeededPanelmembers - genderCount;
            if (neededGender % 2 == 0 || neededGender > 1)
            {
                maleCount++;
                femaleCount++;
                genderCount+=2;
            }
            else
            {
                maleCount++;
                genderCount+=2;
            }
        }
        
        var ageCount = age18_25Count + age26_40Count + age41_60Count + age60PlusCount;
        while (ageCount < totalNeededPanelmembers)
        {
            var neededAge = totalNeededPanelmembers - ageCount;
            if (neededAge % 4 == 0 || neededAge > 3)
            {
                age18_25Count++;
                age26_40Count++;
                age41_60Count++;
                age60PlusCount++;
                ageCount += 4;
            }
            else if (neededAge % 4 == 3)
            {
                age18_25Count++;
                age26_40Count++;
                age41_60Count++;
                ageCount+=4;
            }
            else if (neededAge % 4 == 2)
            {
                age18_25Count++;
                age26_40Count++;
                ageCount+=4;
            }
            else
            {
                age18_25Count++;
                ageCount+=4;
            }

        }
        
        var result = new RecruitmentResult
        {
            MaleCount = maleCount,
            FemaleCount = femaleCount,
            Age18_25Count = age18_25Count,
            Age26_40Count = age26_40Count,
            Age41_60Count = age41_60Count,
            Age60PlusCount = age60PlusCount,
            ReservePotPanelmembers = reservePotPanelmembers,
            TotalNeededPanelmembers = totalNeededPanelmembers,
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