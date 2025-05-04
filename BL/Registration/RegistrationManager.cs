using CitizenPanel.BL.Domain.Draw;
using CitizenPanel.BL.Domain.Panel;
using CitizenPanel.BL.Domain.User;
using CitizenPanel.DAL;
using CitizenPanel.DAL.Registration;

namespace CitizenPanel.BL.Registration;

public class RegistrationManager(IPanelManager panelManager,IDrawManager drawManager,IMemberManager memberManager) : IRegistrationManager
{
    public IEnumerable<RecruitmentBucket> AssignActualRegistrationsToBuckets(List<RecruitmentBucket> buckets, List<MemberProfile> profiles)
{
    foreach (var bucket in buckets)
    {
        int matchCount = 0;
        foreach (var profile in profiles)
        {
            bool matches = MatchesBucket(profile, bucket);
            if (matches)
                matchCount++;
        }
        bucket.ActualCount = matchCount;
    }

    return buckets;
}

    private bool MatchesBucket(MemberProfile mp, RecruitmentBucket bucket)
    {
        // Check Gender
        bool hasGenderCriteria = false;
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (sub.Equals("Man", StringComparison.OrdinalIgnoreCase))
            {
                hasGenderCriteria = true;
                if (mp.Gender != Gender.Male)
                    return false;
            }
            else if (sub.Equals("Vrouw", StringComparison.OrdinalIgnoreCase))
            {
                hasGenderCriteria = true;
                if (mp.Gender != Gender.Female)
                    return false;
            }
        }

        // Check Age
        bool hasAgeCriteria = false;
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (IsAgeGroup(sub))
            {
                hasAgeCriteria = true;
                if (!IsInAgeGroup(mp.Age, sub))
                    return false;
            }
        }

        // Check other criteria
        foreach (var sub in bucket.SubCriteriaNames)
        {
            if (hasGenderCriteria && (sub.Equals("Man", StringComparison.OrdinalIgnoreCase) ||
                                       sub.Equals("Vrouw", StringComparison.OrdinalIgnoreCase)))
                continue;

            if (hasAgeCriteria && IsAgeGroup(sub))
                continue;
            
            if (!mp.SelectedCriteria.Any(sc =>
                    sc.Name.Equals(sub, StringComparison.OrdinalIgnoreCase) ||
                    sub.Contains(sc.Name, StringComparison.OrdinalIgnoreCase) ||
                    sc.Name.Contains(sub, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }
        return true;
    }

    private static bool IsAgeGroup(string group)
    {
        group = group.Trim();
        return group.EndsWith("+") || group.Contains("-");
    }

    private bool IsInAgeGroup(int age, string group)
    {
        group = group.Trim();
        // b.v. "60+"
        if (group.EndsWith("+"))
        {
            if (int.TryParse(group.TrimEnd('+'), out var lower))
                return age >= lower;
        }
        // b.v. "18-25"
        else if (group.Contains('-'))
        {
            var parts = group.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 2
                && int.TryParse(parts[0], out var lo)
                && int.TryParse(parts[1], out var hi))
            {
                return age >= lo && age <= hi;
            }
        }
        return false;
    }
    
     public void StartFinalDraw(Panel panel)
    {
        // Haal alle criteria en subcriteria op voor dit panel
        var criteria = panelManager.GetCriteriaAndSubcriteriaWithPanelId(panel.Id);
        
        // Haal alle ingeschreven leden op voor dit panel
        var allMembers = memberManager.GetMembersOfPanelWithCriteria(panel.Id).ToList();
        
        // Bereken de benodigde aantallen voor het panel
        var result = drawManager.CalculateRecruitment(panel.TotalAvailablePotentialPanelmembers, criteria);
        
        // Verdeel de leden in buckets op basis van criteria
        var bucketsWithActuals = AssignActualRegistrationsToBuckets(result.Buckets, allMembers);
        
        // Initialiseer de DrawResult
        var drawResult = new DrawResult
        {
            SelectedMembers = new List<MemberProfile>(),
            ReserveMembers = new List<MemberProfile>(),
            NotSelectedMembers = new List<MemberProfile>(),
            TotalNeededPanelmembers = result.TotalNeededPanelmembers,
            ReservePanelmembers = result.ReservePotPanelmembers
        };
        
        // Bereken hoeveel mensen we nodig hebben uit elke bucket
        int totalSelected = result.TotalNeededPanelmembers;
        int totalReserve = result.ReservePotPanelmembers;
        
        // Maak een set om bij te houden welke leden al geselecteerd zijn
        var selectedMemberIds = new HashSet<int>();
        
        // Random number generator voor de loting
        var random = new Random();
        
        // Selecteer uit elke bucket
        foreach (var bucket in bucketsWithActuals)
        {
            // Bereken hoeveel leden we uit deze bucket nodig hebben
            int targetCount = (int)Math.Ceiling(bucket.Count * ((double)totalSelected / panel.TotalAvailablePotentialPanelmembers));
            int reserveCount = (int)Math.Ceiling(bucket.Count * ((double)totalReserve / panel.TotalAvailablePotentialPanelmembers));
            
            // Vind alle leden die in deze bucket passen
            var eligibleMembers = allMembers
                .Where(m => !selectedMemberIds.Contains(m.Id) && MatchesBucket(m, bucket))
                .ToList();
            
            // Als er niet genoeg leden zijn, gebruik wat we hebben
            targetCount = Math.Min(targetCount, eligibleMembers.Count);
            reserveCount = Math.Min(reserveCount, eligibleMembers.Count - targetCount);
            
            // Shuffle de lijst om willekeurig te selecteren
            var shuffledMembers = eligibleMembers.OrderBy(_ => random.Next()).ToList();
            
            // Selecteer leden voor het panel
            for (int i = 0; i < targetCount && i < shuffledMembers.Count; i++)
            {
                drawResult.SelectedMembers.Add(shuffledMembers[i]);
                selectedMemberIds.Add(shuffledMembers[i].Id);
            }
            
            // Selecteer leden voor de reserve lijst
            for (int i = targetCount; i < targetCount + reserveCount && i < shuffledMembers.Count; i++)
            {
                drawResult.ReserveMembers.Add(shuffledMembers[i]);
                selectedMemberIds.Add(shuffledMembers[i].Id);
            }
        }
        
        // Voeg alle niet-geselecteerde leden toe aan de NotSelectedMembers lijst
        drawResult.NotSelectedMembers = allMembers
            .Where(m => !selectedMemberIds.Contains(m.Id))
            .ToList();
        
        // Update de panel status
        panel.DrawStatus = DrawStatus.Complete;
        panel.DrawResult = drawResult;
        
        // Sla de wijzigingen op in de database
        panelManager.EditPanel(panel);
    }
}