using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using QRCoder;

namespace CitizenPanel.BL;

public class DrawManager : IDrawManager
{
    private readonly Dictionary<int, DrawStatus> _panelDrawStatuses;
    private readonly Dictionary<int, DrawResult> _panelDrawResults;
    private readonly IDrawRepository _drawRepository;
    private readonly IPanelManager _panelManager;

    public DrawManager(IDrawRepository drawRepository, IPanelManager panelManager)
    {
        _drawRepository = drawRepository;
        _panelManager = panelManager;
        
        _panelDrawStatuses = new Dictionary<int, DrawStatus>
        {
            { 1, DrawStatus.FirstPhaseActive },
            { 2, DrawStatus.FirstPhaseActive }
        };
        // Initialize draw results storage
        _panelDrawResults = new Dictionary<int, DrawResult>();

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
            
            string code = GenerateCode(age, genderNumber, panelId, postCode);

            string qrCodePlace = "https://localhost:7145/MemberRegister/RegisterMember?code=" + code; 
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
    
    public string GenerateCode(int age, int gender, int panelId, string postcode)
    {
        Random random = new Random();
        string code = string.Empty;
        string hexAge = age.ToString("X4");
        string hexGender = gender.ToString("X4");
        string hexPanelId = panelId.ToString("X4");
        string hexCode = $"{postcode[0]}-{hexAge}-000{postcode[1]}-{hexGender}-00{postcode[2]}-{hexPanelId}-0{postcode[3]}";
        foreach (char c in hexCode)
        {
            if (c == '0')
            {
                int replaceNumber = random.Next(0, 20);
                char letter = (char)('g' + replaceNumber);
                code += letter;
            }
            else
            {
                code += c;
            }
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

    public RecruitmentResult CalculateRecruitment(RecruitmentCriteria criteria)
    {
        var reservePerc = 0.08;
        var totalAvailablePotPanelmembers = 0.424 * Math.Sqrt(criteria.TotalAvailablePotentialPanelmembers);

        var result = new RecruitmentResult
        {
            MaleCount = (int)(totalAvailablePotPanelmembers * (criteria.MalePercentage / 100)),
            FemaleCount = (int)(totalAvailablePotPanelmembers * (criteria.FemalePercentage / 100)),
            Age18_25Count = (int)(totalAvailablePotPanelmembers * (criteria.Age18_25Percentage / 100)),
            Age26_40Count = (int)(totalAvailablePotPanelmembers * (criteria.Age26_40Percentage / 100)),
            Age41_60Count = (int)(totalAvailablePotPanelmembers * (criteria.Age41_60Percentage / 100)),
            Age60PlusCount = (int)(totalAvailablePotPanelmembers * (criteria.Age60PlusPercentage / 100)),
            ReservePotPanelmembers = (int)(totalAvailablePotPanelmembers / reservePerc),
            TotalNeededPanelmembers = (int)(totalAvailablePotPanelmembers),
            ExtraCriteriaResults = new System.Collections.Generic.List<CriteriaResult>(),
        };

        // Extra criteria (wordt overgenomen zoals eerder)
        foreach (var extra in criteria.ExtraCriteria)
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
    
       public IEnumerable<RecruitmentBucket> GetInvitationBuckets(Panel panel)
    {
        // Define age groups
        var ageGroups = new List<(string Name, int Min, int Max)>
        {
            ("18-25", 18, 25),
            ("26-40", 26, 40),
            ("41-60", 41, 60),
            ("60+", 61, int.MaxValue)
        };
        
        // Group by gender and age group
        var buckets = new List<RecruitmentBucket>();
        foreach (var genderGroup in new[] { Gender.Male, Gender.Female })
        {
            string genderName = genderGroup == Gender.Male ? "Mannen" : "Vrouwen";
            foreach (var ageGroup in ageGroups)
            {
                // Use repository to get count instead of fetching all members
                //var count = _memberRepository.GetMemberCountByPanelIdGenderAndAgeRange(
                   // panel.PanelId, genderGroup, ageGroup.Min, ageGroup.Max);
                
                buckets.Add(new RecruitmentBucket
                {
                    Gender = genderName,
                    AgeGroup = ageGroup.Name,
                    //Count = count
                });
            }
        }
        return buckets;
    }

    public IEnumerable<RecruitmentBucket> GetAllBuckets(Panel panel)
    {
        var existingBuckets = GetInvitationBuckets(panel).ToList();
        var possibleBuckets = new List<RecruitmentBucket>
        {
            new RecruitmentBucket { Gender = "Mannen", AgeGroup = "18-25", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Mannen", AgeGroup = "26-40", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Mannen", AgeGroup = "41-60", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Mannen", AgeGroup = "60+", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "18-25", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "26-40", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "41-60", Count = 0, Target = 5 },
            new RecruitmentBucket { Gender = "Vrouwen", AgeGroup = "60+", Count = 0, Target = 5 }
        };
        
        // Update existing buckets with real values
        foreach (var bucket in possibleBuckets)
        {
            var existingBucket =
                existingBuckets.FirstOrDefault(b => b.Gender == bucket.Gender && b.AgeGroup == bucket.AgeGroup);
            if (existingBucket != null)
            {
                bucket.Count = existingBucket.Count; // Use real count if it exists
            }
        }
        return possibleBuckets.OrderBy(b => b.Gender).ThenBy(b => b.AgeGroup).ToList();
    }

    // Get the current draw status for a panel
    public DrawStatus GetDrawStatus(Panel panel)
    {
        if (_panelDrawStatuses.ContainsKey(panel.PanelId))
        {
            return _panelDrawStatuses[panel.PanelId];
        }
        return DrawStatus.FirstPhaseActive; // Default status
    }

    // Start the final draw for a panel
    public bool StartFinalDraw(Panel panel)
    {
        // Update draw status to complete
        _panelDrawStatuses[panel.PanelId] = DrawStatus.Complete;
        
        // Perform the draw and store results
        var result = PerformFinalDraw(panel);
        
        // Store the results for later retrieval
        _panelDrawResults[panel.PanelId] = result;
        
        return true;
    }

    // Check if there are sufficient registrations for all criteria
    public bool HasSufficientRegistrations(Panel panel)
    {
        var buckets = GetAllBuckets(panel);
        // Check if any bucket has fewer registrations than the target
        return !buckets.Any(b => b.Count < b.Target);
    }

    // Get existing draw results
    public DrawResult GetDrawResults(Panel panel)
    {
        if (_panelDrawResults.ContainsKey(panel.PanelId))
        {
            return _panelDrawResults[panel.PanelId];
        }
        
        // If no results exist, perform the draw now
        var result = PerformFinalDraw(panel);
        
        // Store the results
        _panelDrawResults[panel.PanelId] = result;
        
        return result;
    }

    // Perform the final draw based on criteria
    public DrawResult PerformFinalDraw(Panel panel)
    {
        var buckets = GetAllBuckets(panel);
        var result = new DrawResult();
        var random = new Random();
        
        // Define age ranges for bucket filtering
        var ageRanges = new Dictionary<string, (int Min, int Max)>
        {
            { "18-25", (18, 25) },
            { "26-40", (26, 40) },
            { "41-60", (41, 60) },
            { "60+", (61, int.MaxValue) }
        };

        foreach (var bucket in buckets)
        {
            // Convert string gender to enum
            Gender genderEnum = bucket.Gender == "Mannen" ? Gender.Male : Gender.Female;
            
            // Get age range for this bucket
            var ageRange = ageRanges[bucket.AgeGroup];
            
            // Get all members that match this bucket's criteria using repository
            IEnumerable<Member> members = null;
            var bucketMembers = members; /*_memberManager.GetMembersByPanelIdGenderAndAgeRange(
                panel.PanelId,
                genderEnum,
                ageRange.Min,
                ageRange.Max
            ).ToList();*/
            
            // Randomly select members
            var shuffledMembers = bucketMembers.OrderBy(x => random.Next()).ToList();
            
            // Determine how many to select as main members and how many as reserves
            int mainCount = 0; //Math.Min(bucket.Target, shuffledMembers.Count);
            int reserveCount = Math.Max(0, Math.Min(2, shuffledMembers.Count - mainCount)); // Up to 2 reserve members
            
            // Select main members
            var selectedMembers = new List<Member>();
            for (int i = 0; i < mainCount; i++)
            {
                if (i < shuffledMembers.Count)
                {
                    var member = shuffledMembers[i];
                    member.IsSelected = true;
                    result.SelectedMembers.Add(member);
                    selectedMembers.Add(member);
                }
            }
            
            // Update selected members in the repository
            
            //_memberManager.MarkMembersAsSelected(selectedMembers);
            
            // Select reserve members
            for (int i = mainCount; i < mainCount + reserveCount; i++)
            {
                if (i < shuffledMembers.Count)
                {
                    result.ReserveMembers.Add(shuffledMembers[i]);
                }
            }
        }
        return result;
    }
}