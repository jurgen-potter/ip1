using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Recruitment;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.BL.Domain.PanelManagement;
using CitizenPanel.DAL;
using Microsoft.AspNetCore.Identity;

namespace CitizenPanel.BL;



public class RegistrationManager : IRegistrationManager
{
    private readonly IMemberRepository _memberRepository;
    private readonly Dictionary<int, DrawStatus> _panelDrawStatuses;
    private readonly Dictionary<int, DrawResult> _panelDrawResults;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IPanelManager _panelManager;

    public RegistrationManager(IMemberRepository memberRepository, UserManager<IdentityUser> userManager, IPanelManager panelManager)
    {
        _memberRepository = memberRepository;
        _userManager = userManager;
        _panelManager = panelManager;
        
        // Initialize draw statuses for panels
        _panelDrawStatuses = new Dictionary<int, DrawStatus>
        {
            { 1, DrawStatus.FirstPhaseActive },
            { 2, DrawStatus.FirstPhaseActive }
        };
        // Initialize draw results storage
        _panelDrawResults = new Dictionary<int, DrawResult>();
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
                var count = _memberRepository.GetMemberCountByPanelIdGenderAndAgeRange(
                    panel.PanelId, genderGroup, ageGroup.Min, ageGroup.Max);
                
                buckets.Add(new RecruitmentBucket
                {
                    Gender = genderName,
                    AgeGroup = ageGroup.Name,
                    Count = count
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
            var bucketMembers = _memberRepository.GetMembersByPanelIdGenderAndAgeRange(
                panel.PanelId, 
                genderEnum, 
                ageRange.Min, 
                ageRange.Max
            ).ToList();
            
            // Debug: Print the number of members found for this bucket
            Console.WriteLine($"Bucket {bucket.Gender} {bucket.AgeGroup}: Found {bucketMembers.Count} members");
            
            // Randomly select members
            var shuffledMembers = bucketMembers.OrderBy(x => random.Next()).ToList();
            
            // Determine how many to select as main members and how many as reserves
            int mainCount = Math.Min(bucket.Target, shuffledMembers.Count);
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
            _memberRepository.MarkMembersAsSelected(selectedMembers);
            
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
    
    public async Task<(IdentityResult result, IdentityUser user)> AddMemberAsync(string newMemberFirstName, string newMemberLastName, string newMemberEmail, string newMemberPassword, Gender newMemberGender, DateOnly newMemberBirthDate, string newMemberTown, List<int> newMemberSelectedCriteria, int newMemberPanelId)
    {
        List<SubCriteria> selectedCriteria = new List<SubCriteria>();
        foreach (var criteria in newMemberSelectedCriteria)
        {
            var crit = _panelManager.GetSubCriteria(criteria);
            selectedCriteria.Add(crit);
        }
        
        Member member = new Member()
        {
            FirstName = newMemberFirstName,
            LastName = newMemberLastName,
            Email = newMemberEmail,
            UserName = newMemberEmail,
            Gender = newMemberGender,
            BirthDate = newMemberBirthDate,
            Town = newMemberTown,
            SelectedCriteria = selectedCriteria,
            Panel = _panelManager.GetPanel(newMemberPanelId)
        };
        
        var result = await _userManager.CreateAsync(member, newMemberPassword);

        return result.Succeeded ? (result, member) : (result, null);
    }

}