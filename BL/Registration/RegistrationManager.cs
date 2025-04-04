using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using CitizenPanel.DAL.Registration;

namespace CitizenPanel.BL.Registration;

public class RegistrationManager(IMemberManager memberManager, IPanelManager panelManager) : IRegistrationManager
{
    public IEnumerable<RecruitmentBucket> GetInvitationBuckets(Panel panel)
    {
        // Dit moet nog gekoppelt worden aan de criteria van het panel zelf
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
                var count = memberManager.GetMemberCountByPanelIdGenderAndAgeRange(
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

        var targetBuckets = panelManager.GetTargetBucketsByPanel(panel);
        
        
        // // Update existing buckets with real values
        foreach (var bucket in targetBuckets)
        {
            var existingBucket =
                existingBuckets.FirstOrDefault(b => b.Gender == bucket.Gender && b.AgeGroup == bucket.AgeGroup);
            if (existingBucket != null)
            {
                bucket.Count = existingBucket.Count; // Use real count if it exists
            }
        }
        return targetBuckets.OrderBy(b => b.Gender).ThenBy(b => b.AgeGroup).ToList();
    }

    // Get the current draw status for a panel
    public DrawStatus GetDrawStatus(Panel panel)
    {
        return panel.DrawStatus;
    }

    // Start the final draw for a panel
    public void StartFinalDraw(Panel panel)
    {
        // Perform the draw
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
            Gender genderEnum = bucket.Gender == "Mannen" ? Gender.Male : Gender.Female;
            var ageRange = ageRanges[bucket.AgeGroup];

            var bucketMembers = memberManager.GetMembersByPanelIdGenderAndAgeRange(
                panel.PanelId,
                genderEnum,
                ageRange.Min,
                ageRange.Max
            ).ToList();

            var shuffledMembers = bucketMembers.OrderBy(x => random.Next()).ToList();

            int mainCount = Math.Min(bucket.Target, shuffledMembers.Count);
            int reserveCount = (int)Math.Ceiling(mainCount * 0.1); // 10% reserves, afgerond
            
            for (int i = 0; i < shuffledMembers.Count; i++)
            {
                var member = shuffledMembers[i];

                if (i < mainCount)
                {
                    result.SelectedMembers.Add(member);
                }
                else if (i < mainCount + reserveCount)
                {
                    result.ReserveMembers.Add(member);
                }
                else
                {
                    result.NotSelectedMembers.Add(member);
                }
            }

            panel.DrawResult = result;

            panel.DrawStatus = DrawStatus.Complete;

            panelManager.EditPanel(panel);
        }
    }

    // Check if there are sufficient registrations for all criteria
    public bool HasSufficientRegistrations(Panel panel)
    {
        var buckets = GetAllBuckets(panel);
        // Check if any bucket has fewer registrations than the target
        return !buckets.Any(b => b.Count < b.Target);
    }
}