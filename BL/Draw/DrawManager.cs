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
            int panelId = dummyMember.PanelId;
            string town = dummyMember.Town;
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

    public IEnumerable<Invitation> AddInvitations(Panel panel, List<DummyMember> members)
    {
        QRCodeGenerator qrGenerator = new QRCodeGenerator();
        List<Invitation> invitations = new List<Invitation>();

        foreach (Criteria criteria in panel.Criteria)
        {
            foreach (SubCriteria subCriteria in criteria.SubCriteria)
            {
                
            }
        }
        
        foreach (DummyMember dummyMember in members)
        {
            int age = dummyMember.Age;
            Gender gender = dummyMember.Gender;
            int panelId = dummyMember.PanelId;
            string town = dummyMember.Town;
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


    public Criteria GetCriteria(int criteriaId)
    {
        return _drawRepository.ReadCriteria(criteriaId);
    }

    public IEnumerable<Criteria> GetAllCriteria()
    {
        return _drawRepository.ReadAllCriteria();
    }

    public SubCriteria GetSubCriteria(int subCriteriaId)
    {
        return _drawRepository.ReadSubCriteria(subCriteriaId);
    }

    public IEnumerable<Criteria> GetCriteriaByPanel(int panelId)
    {
        return _drawRepository.ReadCriteriaByPanel(panelId);
    }

    
    public Criteria AddCriteria(string name, List<SubCriteria> subCriteria)
    {
        Criteria criteria = new Criteria()
        {
            Name = name,
            SubCriteria = subCriteria
        };
        return _drawRepository.CreateCriteria(criteria);
    }

    public SubCriteria AddSubCriteria(string name, double percentage)
    {
        SubCriteria subCriteria = new SubCriteria()
        {
            Name = name,
            Percentage = percentage
        };
        return _drawRepository.CreateSubCriteria(subCriteria);
    }
    
    public void EditCriteria(int panelId, IEnumerable<Criteria> criteria)
    {
        _drawRepository.UpdateCriteria(panelId, criteria);
    }


    public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, IEnumerable<Criteria> criteriaList)
{
    // Bepaal totale aantallen
    int totalToDraw = (int)Math.Round(0.50 * Math.Sqrt(totalAvailablePotentialPanelmembers));
    int reservePool = (int)Math.Ceiling(totalToDraw / 0.08);

    // Initialiseer result
    var result = new RecruitmentResult
    {
        TotalNeededPanelmembers = totalToDraw,
        ReservePotPanelmembers = reservePool,
        CriteriaResults = new List<CriteriaResult>(),
        Buckets = new List<RecruitmentBucket>()
    };

    // Vullen van CriteriaResults 
    foreach (var crit in criteriaList)
    {
        var critResult = new CriteriaResult
        {
            Name = crit.Name,
            SubResults = new List<SubCriteriaResult>()
        };

        foreach (var sub in crit.SubCriteria)
        {
            critResult.SubResults.Add(new SubCriteriaResult
            {
                Name = sub.Name,
                Count = (int)Math.Round(totalToDraw * (sub.Percentage / 100.0))
            });
        }

        result.CriteriaResults.Add(critResult);
    }

    // Buckets genereren via helper methode op basis van originele criteriaList
    result.Buckets = BuildBuckets(criteriaList.ToList(), totalToDraw);

    // Correcties om tot precies TotalNeededPanelmembers te komen
    int totalCount = result.Buckets.Sum(b => b.Count);
    while (totalCount < result.TotalNeededPanelmembers)
    {
        var bucket = result.Buckets.OrderBy(b => b.Count).First();
        bucket.Count++;
        totalCount++;
    }
    while (totalCount > result.TotalNeededPanelmembers)
    {
        var bucket = result.Buckets.OrderByDescending(b => b.Count).First();
        bucket.Count--;
        totalCount--;
    }

    return result;
}

private static List<RecruitmentBucket> BuildBuckets(
    List<Criteria> criteriaList,
    int totalToDraw)
{
    var buckets = new List<RecruitmentBucket>();

    void Recurse(int depth, List<string> chosenCriteria, List<string> chosenSubs, double accumulatedPct)
    {
        if (depth == criteriaList.Count)
        {
            int count = (int)Math.Round(totalToDraw * accumulatedPct);
            buckets.Add(new RecruitmentBucket
            {
                CriteriaNames = new List<string>(chosenCriteria),
                SubCriteriaNames = new List<string>(chosenSubs),
                Count = count
            });
            return;
        }

        var current = criteriaList[depth];
        foreach (var sub in current.SubCriteria)
        {
            chosenCriteria.Add(current.Name);
            chosenSubs.Add(sub.Name);

            Recurse(
                depth + 1,
                chosenCriteria,
                chosenSubs,
                accumulatedPct * (sub.Percentage / 100.0));

            chosenCriteria.RemoveAt(chosenCriteria.Count - 1);
            chosenSubs.RemoveAt(chosenSubs.Count - 1);
        }
    }

    // Start recursie met lege lijsten en 100% (1.0)
    Recurse(0, new List<string>(), new List<string>(), 1.0);

    return buckets;
}



    // public RecruitmentResult CalculateRecruitment(int totalAvailablePotentialPanelmembers, double malePercentage, double femalePercentage, double age18_25Percentage, double age26_40Percentage, double age41_60Percentage, double age60PlusPercentage, List<Criteria> extraCriteria)
    // {
    //     var reservePerc = 0.08;
    //     var totalAvailablePotPanelmembers = Math.Round(0.50 * Math.Sqrt(totalAvailablePotentialPanelmembers));
    //
    //
    //     var maleCount = (int)(totalAvailablePotPanelmembers * (malePercentage / 100));
    //     var femaleCount = (int)(totalAvailablePotPanelmembers * (femalePercentage / 100));
    //     var age18_25Count = (int)(totalAvailablePotPanelmembers * (age18_25Percentage / 100));
    //     var age26_40Count = (int)(totalAvailablePotPanelmembers * (age26_40Percentage / 100));
    //     var age41_60Count = (int)(totalAvailablePotPanelmembers * (age41_60Percentage / 100));
    //     var age60PlusCount = (int)(totalAvailablePotPanelmembers * (age60PlusPercentage / 100));
    //     var reservePotPanelmembers = (int)(totalAvailablePotPanelmembers / reservePerc);
    //     var totalNeededPanelmembers = (int)(totalAvailablePotPanelmembers);
    //
    //     var genderCount = maleCount + femaleCount;
    //     while (genderCount < totalNeededPanelmembers)
    //     {
    //         var neededGender = totalNeededPanelmembers - genderCount;
    //         if (neededGender % 2 == 0 || neededGender > 1)
    //         {
    //             maleCount++;
    //             femaleCount++;
    //             genderCount+=2;
    //         }
    //         else
    //         {
    //             maleCount++;
    //             genderCount+=2;
    //         }
    //     }
    //     
    //     var ageCount = age18_25Count + age26_40Count + age41_60Count + age60PlusCount;
    //     while (ageCount < totalNeededPanelmembers)
    //     {
    //         var neededAge = totalNeededPanelmembers - ageCount;
    //         if (neededAge % 4 == 0 || neededAge > 3)
    //         {
    //             age18_25Count++;
    //             age26_40Count++;
    //             age41_60Count++;
    //             age60PlusCount++;
    //             ageCount += 4;
    //         }
    //         else if (neededAge % 4 == 3)
    //         {
    //             age18_25Count++;
    //             age26_40Count++;
    //             age41_60Count++;
    //             ageCount+=4;
    //         }
    //         else if (neededAge % 4 == 2)
    //         {
    //             age18_25Count++;
    //             age26_40Count++;
    //             ageCount+=4;
    //         }
    //         else
    //         {
    //             age18_25Count++;
    //             ageCount+=4;
    //         }
    //
    //     }
    //     
    //     var result = new RecruitmentResult
    //     {
    //         MaleCount = maleCount,
    //         FemaleCount = femaleCount,
    //         Age18_25Count = age18_25Count,
    //         Age26_40Count = age26_40Count,
    //         Age41_60Count = age41_60Count,
    //         Age60PlusCount = age60PlusCount,
    //         ReservePotPanelmembers = reservePotPanelmembers,
    //         TotalNeededPanelmembers = totalNeededPanelmembers,
    //         CriteriaResults = new List<CriteriaResult>()
    //     };
    //
    //     
    //     // Extra criteria (wordt overgenomen zoals eerder)
    //     foreach (var extra in extraCriteria)
    //     {
    //         var criteriaResult = new CriteriaResult
    //         {
    //             Name = extra.Name,
    //             SubResults = extra.SubCriteria.Select(sub => new SubCriteriaResult
    //             {
    //                 Name = sub.Name,
    //                 Count = (int)(totalAvailablePotPanelmembers * (sub.Percentage / 100))
    //             }).ToList()
    //         };
    //         result.CriteriaResults.Add(criteriaResult);
    //     }
    //
    //     return result;
    // }
    public bool RemoveInvitation(int invitationId)
    {
        return _drawRepository.DeleteInvitation(invitationId);
    }
    public IEnumerable<Criteria> GetInitialCriteria()
    {
        var criteriaList = new List<Criteria>()
        {
            AddCriteria("Geslacht",
            [
                AddSubCriteria("Man", 0),
                AddSubCriteria("Vrouw", 0)
            ]),
            AddCriteria("Leeftijd",
            [
                AddSubCriteria("18-25", 0),
                AddSubCriteria("26-35", 0),
                AddSubCriteria("36-50", 0),
                AddSubCriteria("51-60", 0),
                AddSubCriteria("60+", 0),
            ])
        };
        
        return criteriaList;
    }
}